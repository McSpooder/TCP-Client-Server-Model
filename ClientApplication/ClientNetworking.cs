using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;


//demonstrates sockets

namespace location
{

    class ClientNetworking
    {

        static void ExecuteClient()
        {

        }
        static void Main(string[] args)
        {


            if (args.Length == 0)
            {

            }
            if (args.Length > 0)
            {
                //default values are set here
                string server = "whois.net.dcs.hull.ac.uk";
                int port = 43;
                int timeOut = 1000;
                string protocol = "-w"; //protocol are either: whois = w, http0.9 = h9, http1.0 = h0, http1.1 = h1
                bool debug = false;

                List<string> argsList = args.ToList(); //turns creates a list for arguments, this is so values can easily be removed without leaving empty indexes.

                //This for loop goes through the arguments list, to determine if any default program values should be changed.
                //It also removes the flags and their relevent data if they are contained in the list. The flag data is always after the flag specifier.
                //The end result is only the name and/or location.
                int i = 0;
                while (i < argsList.Count)
                {
                    switch (argsList[i])
                    {
                        case "-h":
                            server = args[i + 1];
                            argsList.RemoveAt(i);
                            argsList.RemoveAt(i + 1);
                            break;
                        case "-p":
                            port = int.Parse(args[i + 1]);
                            argsList.RemoveAt(i);
                            argsList.RemoveAt(i + 1);
                            break;
                        case "-t":
                            timeOut = int.Parse(args[i + 1]);
                            argsList.RemoveAt(i);
                            argsList.RemoveAt(i + 1);
                            break;
                        case "-h9":
                            protocol = "-h9";
                            argsList.RemoveAt(i);
                            break;
                        case "-h0":
                            protocol = "-h0";
                            argsList.RemoveAt(i);
                            break;
                        case "-h1":
                            protocol = "-h1";
                            argsList.RemoveAt(i);
                            break;
                        case "-d":
                            debug = true;
                            argsList.RemoveAt(i);
                            break;
                        default:
                            i++; //if an argument wasnt removed then then next index should be checked, otherwise it will be the same argument being checked.
                            break;
                    }

                }

                if (debug)
                {
                    Console.WriteLine();
                    Console.WriteLine("=============== DEBUG ===============");
                    Console.WriteLine("Server: {}\nPort: {}\nTime Out: {}", server, port, timeOut);

                }

                TcpClient client = new TcpClient();
                client.Connect(server, port);
                StreamWriter sw = new StreamWriter(client.GetStream());
                StreamReader sr = new StreamReader(client.GetStream());
                client.ReceiveTimeout = timeOut;
                client.SendTimeout = timeOut;


                if (protocol == "-w")
                {
                    if (debug)
                    {
                        Console.WriteLine("Protocol: WHOIS");
                    }

                    if (argsList.Count == 1)
                    {
                        if (debug)
                        {
                            Console.WriteLine("Command: Retrieve");
                            Console.WriteLine("Container name: " + argsList[0]);
                            Console.WriteLine("=============== END DEBUG ===============");
                        }

                        sw.WriteLine(argsList[0]);
                        sw.Flush();
                        Console.Write(argsList[0] + " is ");
                        Console.WriteLine(sr.ReadToEnd());
                    }
                    else if (argsList.Count == 2)
                    {
                        if (debug)
                        {
                            Console.WriteLine("Command: Update");
                            Console.WriteLine("Container name: " + argsList[0]);
                            Console.WriteLine("Container value: " + argsList[1]);
                            Console.WriteLine("=============== END DEBUG ===============");
                        }

                        sw.WriteLine(argsList[0] + " " + argsList[1]);
                        sw.Flush();
                        sr.ReadToEnd(); //Reading the server acknoledgement, otherwise server database doesnt update
                        Console.WriteLine(argsList[0] + " location changed to be " + argsList[1]);
                    }
                }
                else if (protocol == "-h9")
                {
                    if (debug)
                    {
                        Console.WriteLine("Protocol: HTTP 0.9");
                    }

                    if (argsList.Count == 1)
                    {
                        if (debug)
                        {
                            Console.WriteLine("Command: Retrieve");
                            Console.WriteLine("Container name: " + argsList[0]);
                            Console.WriteLine("=============== END DEBUG ===============");
                        }

                        sw.WriteLine("GET /" + args[0]);
                        sw.Flush();

                        string serverResponse = sr.ReadLine();

                        if (serverResponse.Contains("OK"))
                        {
                            sr.ReadLine(); //Content-Type:<space>text/plain<CR><LF>
                            sr.ReadLine(); //<CR><LF>
                            Console.WriteLine(args[0] + " is " + sr.ReadLine());
                        }
                        else
                        {
                            Console.WriteLine("Name not found in database.");
                        }

                    }
                    else //2 arguments, meaning its an update command
                    {
                        if (debug)
                        {
                            Console.WriteLine("Command: Update");
                            Console.WriteLine("Container name: " + argsList[0]);
                            Console.WriteLine("Container value: " + argsList[1]);
                            Console.WriteLine("=============== END DEBUG ===============");
                        }

                        sw.WriteLine("PUT /" + args[0]);
                        sw.WriteLine();
                        sw.WriteLine(args[1]);
                        sw.Flush();

                        string serverResponse = sr.ReadLine();

                        if (serverResponse.Contains("OK"))
                        {
                            sr.ReadLine();
                            sr.ReadLine();
                            Console.WriteLine(args[0] + " location changed to be " + args[1]);
                        }
                        else
                        {
                            Console.WriteLine("Something went wrong.");
                        }

                    }
                }
                else if (protocol == "-h0")
                {
                    if (debug)
                    {
                        Console.WriteLine("Protocol: HTTP 1.0");
                    }

                    if (argsList.Count == 1)
                    {
                        if (debug)
                        {
                            Console.WriteLine("Command: Retrieve");
                            Console.WriteLine("Container name: " + argsList[0]);
                            Console.WriteLine("=============== END DEBUG ===============");
                        }

                        sw.WriteLine("GET /?" + args[0] + " HTTP/1.0");
                        sw.Flush();

                        string serverResponse = sr.ReadLine();

                        if (serverResponse.Contains("OK"))
                        {
                            sr.ReadLine();
                            sr.ReadLine();
                            Console.WriteLine(args[0] + " is " + sr.ReadLine());
                        }
                        else
                        {
                            Console.WriteLine("Name not found in database.");
                        }

                    }
                    else
                    {
                        if (debug)
                        {
                            Console.WriteLine("Command: Update");
                            Console.WriteLine("Container name: " + argsList[0]);
                            Console.WriteLine("Container value: " + argsList[1]);
                            Console.WriteLine("=============== END DEBUG ===============");
                        }


                        sw.WriteLine("POST /" + args[0] + " HTTP/1.0");
                        sw.WriteLine("Content-Length: " + args[1].Length);
                        sw.WriteLine(args[1]);
                        sw.Flush();

                        string serverResponse = sr.ReadLine();

                        if (serverResponse.Contains("OK"))
                        {
                            sr.ReadLine();
                            sr.ReadLine();
                            Console.WriteLine(args[0] + " location changed to be " + args[1]);
                        }
                        else
                        {
                            Console.WriteLine("something went wrong");
                        }


                    }
                }
                else if (protocol == "-h1")
                {
                    if (debug)
                    {
                        Console.WriteLine("Protocol: HTTP 1.1");
                    }

                    if (argsList.Count == 1)
                    {

                        if (debug)
                        {
                            Console.WriteLine("Command: Retrieve");
                            Console.WriteLine("Container name: " + argsList[0]);
                            Console.WriteLine("=============== END DEBUG ===============");
                        }

                        sw.WriteLine("GET /?name=" + args[0] + " HTTP/1.1");
                        sw.WriteLine("Host: Anonymous");
                        sw.Flush();

                        string serverResponse = sr.ReadLine();

                        if (serverResponse.Contains("OK"))
                        {
                            do //reads all the optional header lines
                            {
                                serverResponse = sr.ReadLine();
                            }
                            while (serverResponse != "");

                            serverResponse = sr.ReadLine();
                            Console.WriteLine(args[0] + " is " + serverResponse);
                        }
                        else
                        {
                            Console.WriteLine("Something went wrong.");
                        }

                    }
                    else
                    {
                        if (debug)
                        {
                            Console.WriteLine("Command: Update");
                            Console.WriteLine("Container name: " + argsList[0]);
                            Console.WriteLine("Container value: " + argsList[1]);
                            Console.WriteLine("=============== END DEBUG ===============");
                        }

                        sw.WriteLine("POST / HTTP/1.1");
                        sw.WriteLine("HOST: anonymous");
                        sw.WriteLine("Content-Length: " + args[1].Length);
                        sw.WriteLine("name=" + args[0] + "&location=" + args[1]);
                        sw.Flush();

                        string serverResponse = sr.ReadLine();

                        if (serverResponse.Contains("OK"))
                        {
                            sr.ReadToEnd();
                            Console.WriteLine(args[0] + " location changed to be " + args[1]);
                        }
                        else
                        {
                            Console.WriteLine("Something went wrong.");
                        }


                    }
                }

            }

        }

    }

}