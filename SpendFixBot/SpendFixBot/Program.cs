using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using DataBaseCon = SpendingDatabase.Program;

namespace SpendFixBot
{
    class Program
    {
        private static readonly Telegram.Bot.TelegramBotClient Bot = new TelegramBotClient(SpendFixBot.Config.API_KEY);

        static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;

            var me = Bot.GetMeAsync().Result;
            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage)
                return;

            if (message.Text.StartsWith("/help"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, SpendFixBot.Constants.HELP_MESSAGE);
            }

            var keyboardTypeOfOutput = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup();
            keyboardTypeOfOutput.InlineKeyboard = new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][]
            {
                    new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[]
                    {
                        new KeyboardButton("по категориям"),
                        new KeyboardButton("по датам"),
                    },
            };

            if (message.Text.StartsWith("/spending"))
            {
                Bot.OnCallbackQuery += (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
                {
                    if (ev.CallbackQuery.Data == "по категориям")
                    {
                        // In this plase will be used API to work with the database(SelectRowsByCategory) 
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);
                    }
                    if (ev.CallbackQuery.Data == "по датам")
                    {
                        // In this plase will be used API to work with the database(SelectRowsByDates)
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);
                    }
                };
                await Bot.SendTextMessageAsync(message.Chat.Id, "Выбери вид отчетности", replyMarkup: keyboardTypeOfOutput);
            }

            if (message.Text.StartsWith("/start"))
            {
                DataBaseCon.InsertUser((int)message.Chat.Id, message.Chat.FirstName);
                await Bot.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.Chat.FirstName}" + SpendFixBot.Constants.START_MESSAGE);
            }
            if (message.Text.StartsWith("/delete"))
            {
                // In this plase will be used API to work with the database(DeleteUser) 
                await Bot.SendTextMessageAsync(message.Chat.Id, SpendFixBot.Constants.DELETE_DONE);
            }
        }
    }
}
