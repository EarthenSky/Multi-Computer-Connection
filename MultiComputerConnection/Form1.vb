Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.IO

Public Class Form1
    'Gabe Stang
    '
    'Connect 3 computers together
    'Can connect multiple computers (3) together and can ping name to console.

    Private listener As New TcpListener(5019)
    Private client As New TcpClient

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim ListenerThread As New Thread(New ThreadStart(AddressOf Listening))
        Me.Text += My.Computer.Name
        ListenerThread.Start()
    End Sub

    Private Sub Listening()  'starts listening for info from other com.
        listener.Start()
    End Sub

    'Get info from other computers here.
    Private Sub tmrListenerUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrListenerUpdate.Tick
        If listener.Pending = True Then  'If a Computer wants to send something.
            client = listener.AcceptTcpClient() 'Accepts the "message."

            Dim shtInfo As String = ""

            Dim Reader As New StreamReader(client.GetStream())  'Start getting the other com info.
            While Reader.Peek > -1  'Changes each character sent into a string and adds it to the 
                shtInfo = shtInfo + Convert.ToChar(Reader.Read()).ToString
            End While

            lbxChatConsole.Items.Add(shtInfo)
        End If
    End Sub

    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        client = New TcpClient(tbxConnectionComputerName.Text, 5019)
    End Sub
End Class
