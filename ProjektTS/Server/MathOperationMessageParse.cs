﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class MathOperationMessageParse : ReceivedParseInterface
    {
        static int id = 0;
        Client.SimpleMessage simpleMessage;
        TcpServer tcpServer;
        public MathOperationMessageParse(TcpServer server) { tcpServer = server; }
        IEnumerable<double> PairOperate(List<double> numbers, Func<double, double, double> func)
        {
            if (numbers.Count >= 1)
            {
                IEnumerable<double> pary = numbers.Take(2);
                IEnumerable<double> array;
                do
                {
                    array = numbers.Skip(2);
                    yield return func(pary.First(), pary.Last());
                    pary = array.Take(2);
                } while (pary.Count() == 2);
            }
        }
        public string parseReceived(string message)
        {
            simpleMessage = new Client.SimpleMessage(message);
            simpleMessage.status = "0";
            List<double> result = new List<double>();
            switch (simpleMessage.operation)
            {
                case "dodaj": {
                        double suma = 0;
                        try
                        {
                            foreach (double i in simpleMessage.numbers) { suma += i; }
                            result.Add(suma);
                        }
                        catch(ArgumentOutOfRangeException e)
                        {
                            simpleMessage.status = "1";
                        }
                    } break;
                case "odejmij":
                    {
                        result.AddRange(PairOperate(simpleMessage.numbers, (x, y) => x - y));
                    } break;
                case "mnozenie": {
                        double iloczyn = 1;
                        try
                        {
                            foreach (double i in simpleMessage.numbers) { iloczyn *= i; }
                            result.Add(iloczyn);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            simpleMessage.status = "1";
                        }

                    } break;
                case "dzielenie": {
                        try
                        {
                            result.AddRange(PairOperate(simpleMessage.numbers, (x, y) => x / y));
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            simpleMessage.status = "1";
                        }
                    } break;
                case "log":
                case "logorytm": {
                        result.AddRange(PairOperate(simpleMessage.numbers, (x, y) => Math.Log(y,x)));
                    } break;
                case "modulo": {
                        result.AddRange(PairOperate(simpleMessage.numbers, (x, y) => x % y));
                    } break;
                case "potega": {
                        try
                        {
                            result.AddRange(PairOperate(simpleMessage.numbers, (x, y) => Math.Pow(x, y)));
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            simpleMessage.status = "1";
                        }
                    } break;
                case "pierwiastek": {
                        
                        result.AddRange(simpleMessage.numbers.Select(x => Math.Sqrt(x)));
                        
                    } break;
                case "exit": {
                        tcpServer.CloseClient();
                        return "";
                    } 
            }
            simpleMessage.numbers = result;
            if (simpleMessage.id == "") { simpleMessage.id = (++id).ToString(); }
            simpleMessage.dateTime = DateTime.Now;

            return simpleMessage.buildMessage();
        }
    }
}
