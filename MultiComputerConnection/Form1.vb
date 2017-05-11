Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.IO

Public Enum Process
    None
    StringStart
    AddComEnd
    MessageEnd
End Enum

Public Class Form1
    'Gabe Stang
    '
    'Connect 3 computers together
    'Can connect multiple computers (3) together and can ping name to console.

    Public Const chrStartProcessingText As Char = Chr(7)
    Public Const chrAddComToConnectListEnd As Char = Chr(6)
    Public Const chrMessageEnd As Char = Chr(5)

    Private listener As New TcpListener(5019)
    Private client As New TcpClient

    Private lstComputers As New List(Of String)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim ListenerThread As New Thread(New ThreadStart(AddressOf Listening))
        tbxConnectionComputerName.Text = My.Computer.Name
        ListenerThread.Start()
    End Sub

    Private Sub Listening()  'starts listening for info from other com.
        listener.Start()
    End Sub

    'Get info from other computers here.
    Private Sub tmrListenerUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrListenerUpdate.Tick
        Try
            If listener.Pending = True Then  'If a Computer wants to send something.
                client = listener.AcceptTcpClient() 'Accepts the "message".

                ProcessInformation(client)

            End If
        Catch ex As Exception
            Console.WriteLine(ex)
            Dim Errorresult As String = ex.Message
            MessageBox.Show(Errorresult & vbCrLf & vbCrLf & "???", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ProcessInformation(ByRef client As System.Net.Sockets.TcpClient)
        Dim shtInfo As String = ""

        Dim Reader As New StreamReader(client.GetStream())  'Start getting the other com info.

        Dim currentProcess As Process = Process.None 'Current thing to do with the recieved information.

        While Reader.Peek > -1  'Changes each character sent into a string and adds it to the 
            Dim chrCurrent As Char = Convert.ToChar(Reader.Read())

            'Set Process if needed
            If chrCurrent = chrStartProcessingText Then
                currentProcess = Process.StringStart
            ElseIf chrCurrent = chrAddComToConnectListEnd Then
                currentProcess = Process.AddComEnd
            ElseIf chrCurrent = chrMessageEnd Then
                currentProcess = Process.MessageEnd
            End If


            If currentProcess = Process.StringStart Then 'Adds chars to be Proccessed
                shtInfo += chrCurrent

            ElseIf currentProcess = Process.AddComEnd Then
                lstComputers.Add(shtInfo)
                shtInfo = String.Empty

            ElseIf currentProcess = Process.MessageEnd Then
                lbxChatConsole.Items.Add(shtInfo.ToString())
                shtInfo = String.Empty

            Else
                lbxChatConsole.Items.Add(" No Understandable Message Header ")

            End If

        End While
    End Sub

    Private Sub btnUpdateChatConsole_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateChatConsole.Click
        Try
            ConnectToComputersAndSendMessages()

            lbxChatConsole.Items.Add(chrStartProcessingText & tbxMessageToSend.Text & chrMessageEnd)
            lbxChatConsole.Items.Add(chrStartProcessingText & My.Computer.Name & chrMessageEnd)

            tbxMessageToSend.Text = "Sent!"
        Catch ex As Exception
            Console.WriteLine(ex)
            Dim Errorresult As String = ex.Message
            MessageBox.Show(Errorresult & vbCrLf & vbCrLf & "Please Review Client Address", "Error Sending Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConnectToComputersAndSendMessages()
        For Each str As String In lstComputers
            client = New TcpClient(str, 5019)

            'Sends the message.
            Dim Writer As New StreamWriter(client.GetStream())
            Writer.Write(
                            chrStartProcessingText & tbxMessageToSend.Text & chrMessageEnd &
                            chrStartProcessingText & My.Computer.Name & chrMessageEnd
                        )
            Writer.Flush()
        Next
    End Sub

    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        lstComputers.Add(tbxConnectionComputerName.Text)
    End Sub
End Class
