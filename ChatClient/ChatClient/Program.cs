using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;


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
        static string getAccessKey(string address, int port, string username, string password)
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
            //Console.WriteLine("we in receiver boiz");
           // Console.WriteLine("Synchro...\n");

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
                    Console.Write(":> ");
                    string msg = Console.ReadLine();
                     writer.Write(IPAddress.HostToNetworkOrder(msg.Length));
                     writer.Write(Encoding.UTF8.GetBytes(msg));
                    //TODO validate
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
        public static void ContinousConnection(String address, int port, string ak)
        {
            Console.WriteLine("Async...\n");
            // Queue<T, T> historyChat = new Queue<T, T>();
             Queue<string> historyChatNames = new Queue<string>();
            Queue<string> historyChatMsgs = new Queue<string>();



            TcpClient client;
            BufferedStream stream;
            BinaryReader reader;
            BinaryWriter writer;
            int hcc = 1;
            try
            {
                client = new TcpClient(address, port);
                stream = new BufferedStream(client.GetStream());
                reader = new BinaryReader(stream);
                writer = new BinaryWriter(stream);

                //if you don't use a using statement, you'll need to flush manually.
                
                stream.Flush();
                //send ak
                //send
                byte[] authBytes = Encoding.UTF8.GetBytes(ak);
                writer.Write(IPAddress.HostToNetworkOrder(ak.Length));
                writer.Write(authBytes);
                Console.WriteLine("ak sent\n");
                
                
            }
            catch(Exception ex)
            {
                Console.WriteLine("error: {0}", ex.Message);
                throw ex;
            }
            while (true) 
            { 
            
                //TODO: do work!if (tcpClient.Connected)

                // stream.BufferSize >0

                /*
                //grab how many messages from broadcaster
                int historyChatLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                byte[] historyChatBytes = reader.ReadBytes(historyChatLength);
                string historyChatCount = Encoding.UTF8.GetString(historyChatBytes);
                hcc = int.Parse(historyChatCount);
                */
              //  while(hcc> 0)
                //{
                   
                // 
                // //grab name and message
                int userMessageLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                byte[] userMessageBytes = reader.ReadBytes(userMessageLength);
                string userMessage = Encoding.UTF8.GetString(userMessageBytes);

                int messageLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                byte[] messageBytes = reader.ReadBytes(messageLength);
                string message = Encoding.UTF8.GetString(messageBytes);

                historyChatNames.Enqueue(userMessage +" says: " + message);
               // historyChatMsgs.Enqueue(message);
                //Console.WriteLine("\n{0} says: {1}", userMessage, message);
                Console.Clear();

                foreach(string  n in historyChatNames)
                {
                    Console.WriteLine("{0}", n);
                }

                Console.WriteLine(":>");

            }


            // 1 add message to queue
            // 2 flush screen
            // 3 display all messages
            //       hcc--;
            // }

            // break;


        }

        static async Task TaskedConnection(string address, int port, string un, string pw)
        {
          //  await Task.Run(() => { getAccessKey(address, port, un, pw); });
        }

        public static void ThreadedConnection(string address, int port, string ak)
        {
            ThreadStart ts = () => { ContinousConnection(address, port, ak); };
            Thread thread = new Thread(ts);
            thread.Start();
            

            //if you want to block until the thread is done, call join. Otherwise, you can
            //just return
            //thread.Join();
            
            //return;
        }
        static void Main(string[] args)
        {
            //adapt code below for comm on port 3461 auth and 3462 receiver

            string address = "hsu.adamcarter.com";
            address = "127.0.0.1";
            int portAuth = 3461; //authentication
            int portReci = 3462; //receiver
            int portBroa = 3463; //broadcast


            // Console.Write("Username: ");
            string username = "yarp"; //Console.ReadLine();
           // Console.Write("Password:keke ");
            string password = "keke"; // Console.ReadLine();

            Console.WriteLine("sending credentials...\n"); //grab our auth key!
            string ak = getAccessKey(address, portAuth, username, password);


            // call receiver
            SynchronousConnectionToReceiver(address, portReci, username, password, ak);
            // call broadcast // call async and have it save to a DS
            // ContinousConnection(address, portBroa, ak);
            ThreadedConnection(address, portBroa, ak);
            while (true)
            {
      
                // call receiver
                SynchronousConnectionToReceiver(address, portReci, username, password, ak);
            }

            /*
            while(true)
            {
                //broadcast messages here
                ThreadedConnection(address, portBroa, ak);
                //address receiver port so we can send messages
                // SynchronousConnectionToReceiver(address, portReci, username, password, ak);
                _ = TaskedConnection(address, portReci, username, password);

            }
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

                while (true)
                {

                    //address receiver port so we can send messages
                    // SynchronousConnectionToReceiver(address, portReci, username, password, ak);
                    _ = TaskedConnection(address, portReci, username, password);

                    //broadcast messages here
                    ContinousConnection(address, portBroa, ak);
                }

            }
            else
            {
                Console.WriteLine("SUCCESS! accesskey = {0}", ak);
                //Console.WriteLine("next show messages?");

                while (true)
                {
                    //grab messages to broadcast

                    //address receiver port so we can send messages
                    _ = TaskedConnection(address, portReci, username, password);

                    //ask for user input
                    //ContinousConnection(address, portBroa, ak);
                    ThreadedConnection(address, portBroa, ak);
                }
            }
            */


        }
    }
}
