Imports System.IO
Imports System.Text

Module Entrypoint

    Sub Main(ByVal ParamArray args As String())
        If args IsNot Nothing Then
            If args.Length > 0 Then
                If IO.File.Exists(args(0)) Then
                    Dim data As Byte() = {}
                    If CrowbarSignature(args(0)) Then
                        Console.WriteLine("Detected Crowbar signature, patching..")
                        If patch(data, args(0)) Then
                            Dim fn As String = args(0).Substring(args(0).LastIndexOf("\") + 1)
                            Dim workingdir As String = args(0).Replace("\" & fn, "")
                            IO.File.WriteAllBytes(Path.Combine(workingdir, "smdpatch_" & fn), data)
                            Console.WriteLine("Patched!")
                        End If
                    Else
                        Console.WriteLine("No Crowbar signature detected.")
                    End If
                End If
            End If
        End If
        Console.ReadLine()
    End Sub

    Function patch(ByRef data As Byte(), ByVal file As String) As Boolean
        Using fs As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
            Using br As New BinaryReader(fs)
                Dim bytes() As Byte = New Byte((br.BaseStream.Length) - 33) {}
                fs.Position = 32
                fs.Read(bytes, 0, bytes.Length)
                data = bytes
                Return True
            End Using
        End Using
    End Function

    Function CrowbarSignature(ByVal file As String) As Boolean
        Using fs As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
            Using br As New BinaryReader(fs)
                Dim bytes(32) As Byte
                br.Read(bytes, 0, bytes.Length)
                Dim str As String = UnChar(bytes)
                Return str.Contains("Crowbar")
            End Using
        End Using
    End Function

    Function UnChar(ByVal Str As Byte()) As String
        Dim sb As New StringBuilder
        For Each C As Byte In Str
            sb.Append(ChrW(C))
        Next
        Return sb.ToString
    End Function

End Module
