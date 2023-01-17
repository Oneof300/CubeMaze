using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    /** 
    *   Important: The Firewall musst be set to let UDP Packages pass. You need to
    *   let Unity receive incoming Massages. Maybe test with Packet Sender.
    *   Type firewall.cpl in cmd to get to the Firewall settings in Windows.#
    *   
    *   Second: you have to set up the right IP on the Arduino for this machine.
    *   You can find out this ip by typing ipconfig into your console.
    **/

    // This is the value that should be read and is in the range of [-1, 1]
    [HideInInspector]
    public Quaternion rotationTarget = default;
    UdpClient clientData;
    // Port to receive on
    public int portData = 2391;
    private int receiveBufferSize = 16;
    public bool showDebug = false;
    IPEndPoint ipEndPointData;
    private object obj = null;
    private System.AsyncCallback AC;
    byte[] receivedBytes;

    void Start()
    {
        InitializeUDPListener();
    }

    void Update() {
        //Debug.Log(transform.rotation);
    }
    public void InitializeUDPListener()
    {
        ipEndPointData = new IPEndPoint(IPAddress.Any, portData);
        clientData = new UdpClient();
        clientData.Client.ReceiveBufferSize = receiveBufferSize;
        clientData.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
        clientData.ExclusiveAddressUse = false;
        clientData.EnableBroadcast = true;
        clientData.Client.Bind(ipEndPointData);
        clientData.DontFragment = true;
        if (showDebug) Debug.Log("BufSize: " + clientData.Client.ReceiveBufferSize);
        AC = new System.AsyncCallback(ReceivedUDPPacket);
        clientData.BeginReceive(AC, obj);
        Debug.Log("UDP - Start Receiving..");
    }

    void ReceivedUDPPacket(System.IAsyncResult result)
    {
        receivedBytes = clientData.EndReceive(result, ref ipEndPointData);

        ParsePacket();

        clientData.BeginReceive(AC, obj);
    } // ReceiveCallBack

    void ParsePacket()
    {
        // Deserialize the data and set the variables
        float x = BitConverter.ToSingle(receivedBytes, 0);
        float y = BitConverter.ToSingle(receivedBytes, 4);
        float z = BitConverter.ToSingle(receivedBytes, 8);
        float w = BitConverter.ToSingle(receivedBytes, 12);
        if(showDebug)Debug.Log("Received x: " + x + " y: " + y + " z: " + z + " w: " + w);

        // Create a new Quaternion rotation using the received values and making right hand coordinates from left hand
        rotationTarget = new Quaternion(y, -z, -x, w);

    }

    void OnDestroy()
    {
        if (clientData != null)
        {
            clientData.Close();
        }
    }
}