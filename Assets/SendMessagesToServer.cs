using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessagesToServer : MonoBehaviour
{
    GameObject waveRig;
    VariablesNiveles variablesNiveles;

    TCPClient tcpClient;

    public int corazones_ant;

    void Start()
    {
        waveRig = GameObject.Find("Wave Rig");
        variablesNiveles = waveRig.GetComponent<VariablesNiveles>();

        tcpClient = GetComponent<TCPClient>();

        corazones_ant = 3;
    }

    void Update()
    {
        if (corazones_ant != variablesNiveles.corazones)
        {
            tcpClient.SendMessageToServer("Corazones:" + variablesNiveles.corazones.ToString());
            corazones_ant = variablesNiveles.corazones;
        }
    }
}
