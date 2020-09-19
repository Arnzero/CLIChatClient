﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ChatServer.Library;

namespace ChatClient
{
    class Program
    {
        //adapt code below for comm on port 3461 and 3462
        //3463 broadcast
        static string SynchronousConnection(string address, int port, string username, string password)
        {
            try
            {
                TcpClient client = new TcpClient(address, port);

                //A using statement should automatically flush when it goes out of scope
                using(BufferedStream stream = new BufferedStream(client.GetStream()))
                {
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);
                    
                   

                    //send auth un and pw
                    writer.Write(IPAddress.HostToNetworkOrder(username.Length));
                    writer.Write(Encoding.UTF8.GetBytes(username));

                    writer.Write(IPAddress.HostToNetworkOrder(password.Length));
                    writer.Write(Encoding.UTF8.GetBytes(password));

                    /*
                    int userNameLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] userNameBytes = reader.ReadBytes(userNameLength);
                    string userName = Encoding.UTF8.GetString(userNameBytes);

                    int passwordLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] passwordBytes = reader.ReadBytes(passwordLength);
                    */
                    //string password = Encoding.UTF8.GetString(passwordBytes);
                   
                    //TODO validate

                    
                    // receive expected key
                    int accesskeyLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    string accesskey = Encoding.UTF8.GetString(reader.ReadBytes(accesskeyLength));
                    //byte[] accesskeyBytes = reader.ReadBytes(accesskeyLength);
                   // string accesskey = Encoding.UTF8.GetString(accesskeyBytes);

                    /*
                    byte[] authBytes = Encoding.UTF8.GetBytes(user.AuthString);
                    writer.Write(IPAddress.HostToNetworkOrder(authBytes.Length));
                    writer.Write(authBytes);
                    */
                    //Console.WriteLine("key is {0},", accesskey);
                    return accesskey;

                    //TODO: do work!
                }
            }catch(Exception ex)
            {
                Console.WriteLine("error: {0}", ex.Message);
                throw ex;
            }

        }
        static void SynchronousConnectionToReceiver(string address, int port, string username, string password, string key)
        {
            try
            {
                TcpClient client = new TcpClient(address, port);

                //A using statement should automatically flush when it goes out of scope
                using (BufferedStream stream = new BufferedStream(client.GetStream()))
                {
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);



                    //send auth key
                    writer.Write(IPAddress.HostToNetworkOrder(key.Length));
                    writer.Write(Encoding.UTF8.GetBytes(key));

                    //send msg !
                    Console.WriteLine("> ");
                    string msg = Console.ReadLine();
                     writer.Write(IPAddress.HostToNetworkOrder(msg.Length));
                     writer.Write(Encoding.UTF8.GetBytes(msg));

                    /*
                    int userNameLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] userNameBytes = reader.ReadBytes(userNameLength);
                    string userName = Encoding.UTF8.GetString(userNameBytes);

                    int passwordLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] passwordBytes = reader.ReadBytes(passwordLength);
                    */
                    //string password = Encoding.UTF8.GetString(passwordBytes);

                    //TODO validate


                    // receive expected key
                    // int accesskeyLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    // string accesskey = Encoding.UTF8.GetString(reader.ReadBytes(accesskeyLength));
                    //byte[] accesskeyBytes = reader.ReadBytes(accesskeyLength);
                    // string accesskey = Encoding.UTF8.GetString(accesskeyBytes);

                    /*
                    byte[] authBytes = Encoding.UTF8.GetBytes(user.AuthString);
                    writer.Write(IPAddress.HostToNetworkOrder(authBytes.Length));
                    writer.Write(authBytes);
                    */
                    //Console.WriteLine("key is {0},", accesskey);


                    //TODO: do work!
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: {0}", ex.Message);
                throw ex;
            }

        }

        //A continous connection is the best approach for communication on 3463
        public static void ContinousConnection(String address, int port)
        {
            TcpClient client;
            BufferedStream stream;
            BinaryReader reader;
            BinaryWriter writer;
            try
            {
                client = new TcpClient(address, port);
                stream = new BufferedStream(client.GetStream());
                reader = new BinaryReader(stream);
                writer = new BinaryWriter(stream);

                //if you don't use a using statement, you'll need to flush manually.
                stream.Flush();
            }
            catch(Exception ex)
            {
                Console.WriteLine("error: {0}", ex.Message);
                throw ex;
            }
            while(true)
            {
                //TODO: do work!
                Console.WriteLine("Username: ");
                string username = Console.ReadLine();
                Console.WriteLine("Password: ");
                string password = Console.ReadLine();

                Console.WriteLine("sending credentials...\n");

                //send auth un and pw
                writer.Write(IPAddress.HostToNetworkOrder(username.Length));
                writer.Write(Encoding.UTF8.GetBytes(username));

                writer.Write(IPAddress.HostToNetworkOrder(password.Length));
                writer.Write(Encoding.UTF8.GetBytes(password));

            }
        }

        static async Task TaskedConnection(string address, int port, string un, string pw)
        {
            await Task.Run(() => { SynchronousConnection(address, port, un, pw); });
        }

        public static void ThreadedConnection(string address, int port)
        {
            ThreadStart ts = () => { ContinousConnection(address, port); };
            Thread thread = new Thread(ts);
            thread.Start();

            //if you want to block until the thread is done, call join. Otherwise, you can
            //just return
            //thread.Join();
        }
        static void Main(string[] args)
        {
            //adapt code below for comm on port 3461 auth and 3462 receiver

            string address = "127.0.0.1";
            int portAuth = 3461; //authentication
            int portReci = 3462; //receiver
            int portBroa = 3463; //broadcast


            Console.WriteLine("Username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Password: ");
            string password = Console.ReadLine();

            Console.WriteLine("sending credentials...\n");
            string ak = SynchronousConnection(address, portAuth, username, password);

            if (ak == "0")
            {
                Console.WriteLine("Intruder detected, calling COCOMO!!");
                Console.WriteLine("Try one more time champ..");
                Console.WriteLine("Username: ");
                username = Console.ReadLine();
                Console.WriteLine("Password: ");
                password = Console.ReadLine();

                Console.WriteLine("sending credentials...\n");
                ak = SynchronousConnection(address, portAuth, username, password);
                Console.WriteLine("SUCCESS! accesskey = {0}", ak);

                //here is where we call receiver?
                SynchronousConnectionToReceiver(address, portReci, username, password, ak);



            }
            else
            {
                Console.WriteLine("SUCCESS! accesskey = {0}", ak);
                Console.WriteLine("next show messages?");

                //here is where we call receiver?
                SynchronousConnectionToReceiver(address, portReci, username, password, ak);

            }


        }
    }
}