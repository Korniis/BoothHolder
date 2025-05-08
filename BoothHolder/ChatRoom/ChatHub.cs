using BoothHolder.IService;
using Microsoft.AspNetCore.SignalR;
using OllamaSharp;
using Serilog;
using System;

namespace BoothHolder.UserApi.ChatRoom
{
    public class ChatHub : Hub
    {
        private readonly OllamaApiClient _ollama;
        private readonly IBoothService _boothService;
        private readonly Chat _chat;


        public ChatHub(IBoothService boothService)
        {
            _boothService = boothService;

            
            var uri = new Uri("http://localhost:11434"); // Ollama 本地地址
            _ollama = new OllamaApiClient(uri)
            {
                SelectedModel = "deepseek-r1:1.5b",

            };
            string AiInfo = boothService.GetAiInfo();


            var systemPrompt = "假设你是一个家具摊位推荐者请为想购买家具顾客推荐下这些"+ AiInfo;
            _chat = new Chat(_ollama, systemPrompt);
        }
        public async Task SendMessage(string prompt)
        {
            try
            {
                Log.Debug("📥 用户输入: {Prompt}", prompt);
                await Clients.Caller.SendAsync("ReceiveToken", $"[🧠 深度思考中…]");

                await foreach (var token in _chat.SendAsync(prompt))
                {
                    Log.Debug("📤 Token: {Token}", token);
                    await Clients.Caller.SendAsync("ReceiveToken", token);
                }

                await Clients.Caller.SendAsync("EndStream");
            }
            catch (Exception ex)
            {
                Log.Error("❌ 发生错误: {Message}", ex.Message);
                await Clients.Caller.SendAsync("ReceiveToken", $"[FATAL ERROR] {ex.Message}");
            }
        }

    }
}
