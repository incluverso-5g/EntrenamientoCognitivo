using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TCPServer : MonoBehaviour
{
    public string Server_ip = "127.0.0.1";
    public int localPort = 3001;
    public int additionalPort = 3002;  // New port for the second server
    public string additionalServerIp = "127.0.0.1";  // IP for the outbound-only server

    private TcpListener server = null;
    private TcpClient client = null;

    private TcpClient additionalClient = null;  // New client for the outbound-only server
    private NetworkStream additionalStream = null;  // Stream for the outbound-only server

    private NetworkStream stream = null;
    private Thread thread;

    public GameObject UriObject;
    private CubeTest referenceUri;

    GameObject waveRig;
    VariablesNiveles variablesNiveles;

    string data = null;
    string response = null;

    private void Start()
    {
        waveRig = GameObject.Find("Wave Rig");
        variablesNiveles = waveRig.GetComponent<VariablesNiveles>();

        print("Empezamos");
        referenceUri = UriObject.GetComponent<CubeTest>();
        referenceUri.enabled = true;
        Server_ip = UriObject.GetComponent<CubeTest>().hmd_ip;
        additionalServerIp = Server_ip;
        Debug.Log("Server IP is: " + Server_ip);
        // Start the main server
        thread = new Thread(new ThreadStart(SetupServer));
        thread.Start();

        // Start the additional server for sending only
        // SetupAdditionalServer();

        // SendDataFromAdditionalServer(variablesNiveles.corazones.ToString());
    }

    private void SetupServer()
    {
        try
        {
            IPAddress localAddr = IPAddress.Parse(Server_ip);
            server = new TcpListener(localAddr, localPort);
            server.Start();

            byte[] buffer = new byte[1024];

            while (true)
            {
                Debug.Log("Main server waiting for connection...");
                client = server.AcceptTcpClient();
                Debug.Log("Main server connected!");

                data = null;
                stream = client.GetStream();

                int i;
                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    data = Encoding.UTF8.GetString(buffer, 0, i);
                    Debug.Log("Main server received: " + data);

                    // Handle data as needed
                    HandleReceivedData(data, stream);
                }
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Debug.Log("Main server SocketException: " + e);
        }
        finally
        {
            server.Stop();
        }
    }

    private void SetupAdditionalServer()
    {
        Debug.Log("Entra additional server");
        try
        {
            additionalClient = new TcpClient();
            additionalClient.Connect(additionalServerIp, additionalPort);
            additionalStream = additionalClient.GetStream();
            Debug.Log("Additional server connected for sending only.");
        }
        catch (SocketException e)
        {
            Debug.Log("Additional server connection failed: " + e);
        }
    }

    private void HandleReceivedData(string data, NetworkStream activeStream)
    {
        while (referenceUri.newUri == "processing")
        {
            Debug.Log("Processing last command, please wait");
        }

        referenceUri.newUri = data;
        Debug.Log("Reference URI at TCP Server: " + referenceUri.newUri);

        if (data == "GetError")
        {
            Debug.Log("Data sent is: " + data);
            //int vidas = 3 - variablesNiveles.erroresNivel;
            response = variablesNiveles.corazones.ToString();
            SendMessageToClient(response, activeStream);
        }
    }

    public void SendMessageToClient(string message, NetworkStream activeStream)
    {
        byte[] msg = Encoding.UTF8.GetBytes(message);
        activeStream.Write(msg, 0, msg.Length);
        Debug.Log("Sent: " + message);
    }

    // Method to send data from the additional server
    public void SendDataFromAdditionalServer(string message)
    {
        if (additionalStream != null)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            additionalStream.Write(msg, 0, msg.Length);
            Debug.Log("Additional server sent: " + message);
        }
        else
        {
            Debug.Log("Additional server stream is not available.");
        }
    }

    private void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
        server?.Stop();
        thread?.Abort();

        additionalStream?.Close();
        additionalClient?.Close();
    }
}
