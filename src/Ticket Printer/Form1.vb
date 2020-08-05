Imports System.Drawing.Printing
Imports com.epson.pos.driver
Imports System.Management

Public Class Form1

    ' Constant variable holding the Printer name.
    Private Const PRINTER_NAME = "EPSON TM-T82 Receipt"

    ' Variables/Objects.
    Dim m_objAPI As New StatusAPI
    Private WithEvents pdPrint As PrintDocument
    Dim isFinish As Boolean
    Dim cancelErr As Boolean
    Dim printStatus As ASB
    Dim isTimeout As Boolean

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '' Open a printer status monitor for the selected printer.
            'm_objAPI.OpenMonPrinter(OpenType.TYPE_PRINTER, PRINTER_NAME)
            '' Associate the created callback function to the event handler of StatusAPI.
            'AddHandler m_objAPI.StatusCallback, AddressOf StatusMonitoring

            printer = New SlipPrinter.TMT82_Printer("EPSON TM-T82 Receipt")
            printer.Open()
        Catch ex As Exception
        End Try

    End Sub

    ' The executed function when the Print button is clicked.
    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click

        Try
            If Not m_objAPI.IsValid Then
                ' Open a printer status monitor for the selected printer.
                If m_objAPI.OpenMonPrinter(OpenType.TYPE_PRINTER, PRINTER_NAME) <> ErrorCode.SUCCESS Then
                    MessageBox.Show("Failed to open printer status monitor.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Sub
                End If
            End If

            isFinish = False
            cancelErr = False
            isTimeout = False

            ' Set the callback function that will monitor printer status.
            If m_objAPI.SetStatusBack() = ErrorCode.SUCCESS Then

                pdPrint = New PrintDocument

                ' Change the printer to the indicated printer.
                pdPrint.PrinterSettings.PrinterName = PRINTER_NAME

                If pdPrint.PrinterSettings.IsValid Then

                    pdPrint.DocumentName = "Testing"
                    ' Start printing
                    pdPrint.Print()

                    Dim iStartTime = DateTime.Now.Ticks / 100000
                    ' Wait until callback function will say that the task is done.
                    ' When done, end the monitoring of printer status.
                    Do
                        If isFinish Then
                            ' End the monitoring of printer status.
                            m_objAPI.CancelStatusBack()
                        End If
                        If iStartTime + 150000 < DateTime.Now.Ticks / 100000 Then
                            isFinish = True
                            isTimeout = True
                        End If
                    Loop While Not isFinish

                    If isTimeout Then
                        ForceResetPrinter()
                    Else
                        ' Display the status/error message.
                        DisplayStatusMessage()

                        ' If roll paper end is detected, cancel the print job
                        If (printStatus And ASB.ASB_PAPER_END) = ASB.ASB_PAPER_END Then
                            CancelPrintJob()
                        End If

                        ' If an error occurred, restore the recoverable error.
                        If cancelErr Then
                            m_objAPI.CancelError()
                        End If
                    End If

                Else
                    MessageBox.Show("Printer is not available.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If

            Else
                MessageBox.Show("Failed to set callback function.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If

        Catch ex As Exception
            MessageBox.Show("Failed to open StatusAPI.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try

    End Sub

    ' The event handler function when pdPrint.Print is called.
    ' This is where the actual printing of sample data to the printer is made.
    Private Sub pdPrint_Print(ByVal sender As System.Object, ByVal e As PrintPageEventArgs) Handles pdPrint.PrintPage
        Dim x, y, lineOffset As Integer

        ' Instantiate font objects used in printing.
        Dim printFont As New Font("Thai Sans Lite", 16, FontStyle.Regular, GraphicsUnit.Point) 'Substituted to FontA Font
        Dim barcodeFont As New Font("Agency FB", 60) 'Substituted to Barcode1 Font

        e.Graphics.PageUnit = GraphicsUnit.Point

        ' Draw the bitmap
        x = 79
        y = 0
        'e.Graphics.DrawImage(pbImage.Image, x, y, pbImage.Image.Width - 13, pbImage.Image.Height - 10)

        ' Print the receipt text
        lineOffset = printFont.GetHeight(e.Graphics) - 4
        x = 10
        y = 25 + lineOffset
        e.Graphics.DrawString("Address: CPF Ë¹Í§¨Í¡", printFont, Brushes.Black, x, y)
        y += lineOffset
        e.Graphics.DrawString("    TEL:   9999-99-9999   ", printFont, Brushes.Black, x, y)
        y += lineOffset
        e.Graphics.DrawString("       " & Now.ToString(), printFont, Brushes.Black, x, y)

        y += lineOffset
        e.Graphics.DrawString("Car: À¹9823", printFont, Brushes.Black, x, y)
        y += lineOffset
        e.Graphics.DrawString("BO23#: 123456789", printFont, Brushes.Black, x, y)

        y += (lineOffset * 2.7) - 30
        e.Graphics.DrawString("µÐ¡ÃéÒ·Ùâ·¹                       180", printFont, Brushes.Black, x, y)
        y += lineOffset
        e.Graphics.DrawString("µÐ¡ÃéÒÈÃÕä·Â                      90", printFont, Brushes.Black, x, y)
        y += lineOffset
        e.Graphics.DrawString("___________________________________", printFont, Brushes.Black, x, y)

        y += lineOffset - 15
        e.Graphics.DrawString("                ", printFont, Brushes.Black, x, y)
        y += lineOffset
        e.Graphics.DrawString("                ", printFont, Brushes.Black, x, y)

        printFont = New Font("Thai Sans Lite", 10, FontStyle.Regular, GraphicsUnit.Point)
        lineOffset = printFont.GetHeight(e.Graphics) - 4
        y += lineOffset - 10
        'e.Graphics.DrawString("Total     $210.00", printFont, Brushes.Black, x - 1, y)

        'printFont = New Font("Microsoft Sans Serif", 10, FontStyle.Regular, GraphicsUnit.Point)
        'lineOffset = printFont.GetHeight(e.Graphics)
        'y += lineOffset + 20
        'e.Graphics.DrawString("Customer's payment         $250.00", printFont, Brushes.Black, x, y)
        'y += lineOffset
        'e.Graphics.DrawString("Change                      $40.00", printFont, Brushes.Black, x, y - 2)

        ' Draw the barcode using the Barcode device font
        y += lineOffset
        Dim sp1 As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
        Dim sp2 As DateTime = New DateTime(3016, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
        Dim t_time As String = (sp2 - sp1).TotalMinutes.ToString("000000000000")
        t_time = "636118033614"
        e.Graphics.DrawString(t_time, barcodeFont, Brushes.Black, x + 11, y + 5)

        y += lineOffset
        e.Graphics.DrawString("               ", printFont, Brushes.Black, x, y - 2)
        y += lineOffset
        e.Graphics.DrawString("               ", printFont, Brushes.Black, x, y - 2)

        ' Indicate that no more data to print, and the Print Document can now send the print data to the spooler.
        e.HasMorePages = False
    End Sub

    ' The callback function that will monitor printer/printing status.
    Public Sub StatusMonitoring(ByVal dwStatus As ASB)

        If (dwStatus And ASB.ASB_PRINT_SUCCESS) = ASB.ASB_PRINT_SUCCESS Then
            printStatus = dwStatus
            isFinish = True

        ElseIf (dwStatus And ASB.ASB_NO_RESPONSE) = ASB.ASB_NO_RESPONSE Or _
                (dwStatus And ASB.ASB_COVER_OPEN) = ASB.ASB_COVER_OPEN Or _
                (dwStatus And ASB.ASB_AUTOCUTTER_ERR) = ASB.ASB_AUTOCUTTER_ERR Or _
                (dwStatus And ASB.ASB_PAPER_END) = ASB.ASB_PAPER_END Then
            printStatus = dwStatus
            isFinish = True
            cancelErr = True

        End If

    End Sub

    Private Sub DisplayStatusMessage()

        If (printStatus And ASB.ASB_PRINT_SUCCESS) = ASB.ASB_PRINT_SUCCESS Then
            MessageBox.Show("Printing complete.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ElseIf (printStatus And ASB.ASB_NO_RESPONSE) = ASB.ASB_NO_RESPONSE Then
            MessageBox.Show("No response.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        ElseIf (printStatus And ASB.ASB_COVER_OPEN) = ASB.ASB_COVER_OPEN Then
            MessageBox.Show("Cover is open.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        ElseIf (printStatus And ASB.ASB_AUTOCUTTER_ERR) = ASB.ASB_AUTOCUTTER_ERR Then
            MessageBox.Show("Autocutter error occurred.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        ElseIf (printStatus And ASB.ASB_PAPER_END) = ASB.ASB_PAPER_END Then
            MessageBox.Show("Roll paper end sensor: paper not present.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        ElseIf (printStatus And ASB.ASB_UNRECOVER_ERR) = ASB.ASB_UNRECOVER_ERR Then
            ' If unrecover is occur then use BiResetPrinter.
            If MessageBox.Show("Anomalous occurrence." & vbLf & "Execute BiResetPrinter.", "Program16", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                If m_objAPI.ResetPrinter() = ErrorCode.SUCCESS Then
                    MessageBox.Show("Reset printer is success.", "Program16")
                Else
                    MessageBox.Show("Failed to reset printer.", "Program16")
                End If
            End If
        End If
    End Sub

    ' Executes the canceling of print job.
    Private Sub CancelPrintJob()
        Dim searchPrintJobs As ManagementObjectSearcher
        Dim printJobCollection As ManagementObjectCollection
        Dim printJob As ManagementObject
        Dim isDeleted As Boolean = False

        searchPrintJobs = New ManagementObjectSearcher("SELECT * FROM Win32_PrintJob")

        printJobCollection = searchPrintJobs.Get

        For Each printJob In printJobCollection
            If System.String.Compare(printJob.Properties("Name").Value.ToString(), PRINTER_NAME) Then
                printJob.Delete()
                isDeleted = True
                Exit For
            End If
        Next

        If isDeleted Then
            MessageBox.Show("Print job cancelled.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Failed to cancel print job.", "Program16", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    ' The executed function when the Close button is clicked.
    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        If m_objAPI.IsValid Then
            ' close the Status Monitor after using the Status API.
            m_objAPI.CloseMonPrinter()
        End If

        Close()
    End Sub

    Private Sub ForceResetPrinter()
        ' If both Printing complete and error are not take then use BiForceResetPrinter.
        If MessageBox.Show("Anomalous occurrence." & vbLf & "Execute BiForceResetPrinter.", "Program16", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            If m_objAPI.ForceResetPrinter() = ErrorCode.SUCCESS Then
                MessageBox.Show("Force reset printer is success.", "Program16")
            Else
                MessageBox.Show("Failed to force reset printer.", "Program16")
            End If
        End If
    End Sub

    Dim printer As SlipPrinter.TMT82_Printer

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        printer.TestPrint()
    End Sub
End Class
