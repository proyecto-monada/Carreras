/* ArduinoConnector by Alan Zucconi
 * http://www.alanzucconi.com/?p=2979
 */
using System;
using System.Collections;
using System.IO.Ports;
using UnityEngine;

public class ControlArduino : MonoBehaviour
{
    private string s;
    public Double x;
    public Double y;
    public Double z;
    /* The serial port where the Arduino is connected. */
    [Tooltip("The serial port where the Arduino is connected")]
    public string port = "COM6";
    /* The baudrate of the serial port. */
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;

    private SerialPort stream;

    public void Open()
    {
        // Opens the serial port
        stream = new SerialPort(port, baudrate);
        stream.ReadTimeout = 50;
        stream.Open();
        //this.stream.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
    }

    public void WriteToArduino(string message)
    {
        // Send the request
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout)
    {
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException)
        {
            return null;
        }
    }


    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            // A single read attempt
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }
    void Update()
    {
        Open();
        //WriteToArduino("PING");
        s = ReadFromArduino(3500);
        if (s.Equals("x"))
        {
            x = Double.Parse(ReadFromArduino(3500));
        }
        if (s.Equals("y"))
        {
            y = Double.Parse(ReadFromArduino(3500));
        }
        if (s.Equals("z"))
        {
            z = Double.Parse(ReadFromArduino(3500));
        }
        //AsynchronousReadFromArduino((string s) => a=int.Parse(s), () => Debug.LogError("Error!"), 10000f);
        Close();
    }

    public void Close()
    {
        stream.Close();
    }
}