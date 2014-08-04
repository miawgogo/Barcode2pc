Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Form1

    ' The delegate
    Delegate Sub AppendTextbox_Delegate(ByVal [textbox] As TextBox, ByVal [text] As String)

    ' The delegates subroutine.
    Private Sub AppendTextbox_ThreadSafe(ByVal [textbox] As TextBox, ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
        ' If these threads are different, it returns true.
        If [textbox].InvokeRequired Then
            Dim MyDelegate As New AppendTextbox_Delegate(AddressOf AppendTextbox_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[textbox], [text]})
        Else
            [textbox].AppendText([text])
        End If
    End Sub


    Private Sub log(ByRef text As String, ByRef iserror As Boolean)
        If iserror = True Then
            AppendTextbox_ThreadSafe(TextBox1, "[Warn] " + text)
        Else
            AppendTextbox_ThreadSafe(TextBox1, "[Info] " + text)
        End If
        AppendTextbox_ThreadSafe(TextBox1, Environment.NewLine)
    End Sub

    Private Sub emulatekeys(ByRef text As String)
        SendKeys.SendWait(text)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        BackgroundWorker1.RunWorkerAsync()
    End Sub
    Private Const listenPort As Integer = 11000

    Private Sub StartListener()
        Dim done As Boolean = False
        Dim listener As New UdpClient(listenPort)
        Dim groupEP As New IPEndPoint(IPAddress.Any, listenPort)
        Try
            While Not done
                log("Waiting for barcode scanner", False)
                Dim bytes As Byte() = listener.Receive(groupEP)
                log("Received broadcast from {0} :" + groupEP.ToString(), False)
                Dim msg As String = Encoding.ASCII.GetString(bytes, 0, bytes.Length)
                log("Barcode contents are: " + msg, True)
                emulatekeys(msg)
                emulatekeys("{ENTER}")
            End While
        Catch e As Exception
            log(e.ToString(), True)
        Finally
            listener.Close()
        End Try
    End Sub 'StartListener
    Private Sub Settingsbutton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Settingsbutton.Click

    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        StartListener()
    End Sub
End Class
