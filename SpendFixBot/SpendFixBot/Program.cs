using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TextDetection;
using DataBaseCon = SpendingDatabase.Program;

namespace SpendFixBot
{
    /*
     * authors Aleks, Nikita
     * 
     */
    class Program
    {
        private static readonly Telegram.Bot.TelegramBotClient Bot = new TelegramBotClient(SpendFixBot.Config.API_KEY);
        
        static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessage += BotOnPhotoReceived;

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
                DataBaseCon.DeleteRows((int)message.Chat.Id);
                await Bot.SendTextMessageAsync(message.Chat.Id, SpendFixBot.Constants.DELETE_DONE);
            }
        }

        private static async void BotOnPhotoReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            try
            {
                if (message == null || message.Type != MessageType.PhotoMessage)
                    return;


                var fileId = message.Photo[message.Photo.Length - 1].FileId;
                var file = await Bot.GetFileAsync(fileId);

                var stream = file.FileStream;

                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup();
                keyboard.InlineKeyboard = new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[][]
                {
                    new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[]
                    {
                        new KeyboardButton("Продукты питания"),
                        new KeyboardButton("Техника"),
                    },
                    new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[]
                    {
                        new KeyboardButton("Транспорт"),
                        new KeyboardButton("Мобильная связь")
                    },
                    new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardButton[]
                    {
                        new KeyboardButton("Другое")
                    },
                };

                await Bot.SendTextMessageAsync(message.Chat.Id, "Выберите категорию товара, который вы приобрели", replyMarkup: keyboard);

                Bot.OnCallbackQuery += (object sc, CallbackQueryEventArgs ev) =>
                {
                    if (ev.CallbackQuery.Data == "Транспорт")
                    {
                        DataBaseCon.InsertCategory((int)message.Chat.Id, "Транспорт");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Расходы на транспорт зафиксированы");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);
                    }
                    else
                    if (ev.CallbackQuery.Data == "Мобильная связь")
                    {
                        DataBaseCon.InsertCategory((int)message.Chat.Id, "Мобильная связь");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Расходы на мобильную связь зафиксированы");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);
                    }
                    else
                    if (ev.CallbackQuery.Data == "Продукты питания")
                    {
                        DataBaseCon.InsertCategory((int)message.Chat.Id, "Продукты питания");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Расходы на продукты питания зафиксированы");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);

                    }
                    else
                    if (ev.CallbackQuery.Data == "Техника")
                    {
                        DataBaseCon.InsertCategory((int)message.Chat.Id, "Техника");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Расходы на технику зафиксированы");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);
                    }
                    else
                    if (ev.CallbackQuery.Data == "Другое")
                    {
                        DataBaseCon.InsertCategory((int)message.Chat.Id, "Другое");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Расходы на другую категорию зафиксированы");
                        Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);
                    }
                };

                await Bot.SendTextMessageAsync(message.Chat.Id, SpendFixBot.Constants.IT_IS_DONE);

            }
            catch (Exception e)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, SpendFixBot.Constants.FAILED);
            }
        }
    }
}
