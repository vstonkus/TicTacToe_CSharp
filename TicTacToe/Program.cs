using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace TicTacToe
    {
    class Program
        {
        private static readonly Action[] actions =
            {
            new Action { letter = 'X', x = 0, y = 1 },
            new Action { letter = 'X', x = 1, y = 1 },
            new Action { letter = 'X', x = 2, y = 1 }
            };

        static void Main(string[] args)
            {
            args.Select(gameId =>
                {
                    Play(gameId).Count();
                    return true;
                }).Count();
            }

        static IEnumerable<bool> Play(string gameId)
            {
            return actions.Select((action, index) =>
                {
                SendBoard(index == 0 ? new [] { action } : new[] { action }.Concat(ReceiveBoard(gameId, 1)),
                          gameId,
                          1).Count();
                //SendBoard(ReceiveBoard(gameId, 2), gameId, 2).Count();
                return true;
                });
            }

        static IEnumerable<bool> SendBoard(IEnumerable<Action> board, string gameId, int player)
            {
            return new[] { new HttpRequestMessage(HttpMethod.Post, $"http://tictactoe.homedir.eu/game/{gameId}/player/{player}")
                {
                    Content = new StringContent(Parser.Encode(board))
                }
            }.Select(
                request =>
                {
                    request.Content.Headers.ContentType.MediaType = "application/bencode+list";
                    request.Content.Headers.ContentType.CharSet = String.Empty;
                    if (!new HttpClient().SendAsync(request).Result.IsSuccessStatusCode)
                        throw new Exception("Board sending failed.");
                    Console.WriteLine("Board was sent succesfully.");
                    return true;
                });
            }

        static IEnumerable<Action> ReceiveBoard(string gameId, int player)
            {
            return new[]
                {new HttpRequestMessage(HttpMethod.Get, $"http://tictactoe.homedir.eu/game/{gameId}/player/{player}")}
                .Select(request => new {Request = request, Client = new HttpClient() })
                .Select(tuple =>
                    {
                    tuple.Request.Headers.AcceptCharset.Clear();
                    tuple.Request.Headers.AcceptEncoding.Clear();
                    tuple.Request.Headers.AcceptLanguage.Clear();
                    tuple.Request.Headers.Accept.Clear();
                    tuple.Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/bencode+list"));
                    Console.WriteLine("Waiting for action.");
                    return tuple.Client.SendAsync(tuple.Request).Result;
                    }).SelectMany(result =>
                        {
                        if (!result.IsSuccessStatusCode)
                            throw new Exception("Board receiving failed.");
                        Console.WriteLine("Board was received succesfully.");
                        return Parser.Decode(result.Content.ReadAsStringAsync().Result);
                        });
            }
        }
    }
