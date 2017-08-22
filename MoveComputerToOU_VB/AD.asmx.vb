Imports System.Web.Services
Imports System.ComponentModel
Imports System.DirectoryServices

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://stealingwithpride.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class AD
    Inherits System.Web.Services.WebService

    Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

#Region "Web Methods"
    ''' <summary>
    ''' Moves a computer to the specified OU
    ''' </summary>
    ''' <param name="Computername">Computername</param>
    ''' <param name="OUPath">LDAP Path to OU</param>
    ''' <returns>True if computer could be moved. False if an error occured</returns>
    ''' <remarks></remarks>
    <WebMethod(Description:="Move computer to the specified OU")> _
    Public Function MoveComputerToOU(ByVal ComputerName As String, ByVal OUPath As String) As Boolean
        logger.Info("Requested to move computer {0} to OU {1}", ComputerName, OUPath)

        Dim Result As Boolean = False

        ' Check if parameters have been supplied
        If Not String.IsNullOrEmpty(ComputerName) AndAlso Not String.IsNullOrEmpty(OUPath) Then
            Dim ComputerEntry As DirectoryEntry
            Dim OUEntry As DirectoryEntry

            ' Get Computer entry
            Try
                Dim ComputerSearcher As New DirectorySearcher(AddLDAP(GetRootDSE))
                ComputerSearcher.Filter = String.Format("(&(objectClass=computer)(cn={0}))", ComputerName)
                ComputerEntry = ComputerSearcher.FindOne().GetDirectoryEntry
            Catch exc As Exception
                logger.DebugException(String.Format("Exception: {0}.", exc.Message), exc)
            End Try

            ' Get OU Entry
            Try
                Dim OUSearcher As New DirectorySearcher(AddLDAP(GetRootDSE))
                OUSearcher.Filter = String.Format("(&(objectClass=organizationalUnit)(distinguishedName={0}))", OUPath)
                OUEntry = OUSearcher.FindOne().GetDirectoryEntry
            Catch exc As Exception
                logger.DebugException(String.Format("Exception: {0}.", exc.Message), exc)
            End Try

            ' Move if appropriate entries have been found
            If Not ComputerEntry Is Nothing AndAlso Not OUEntry Is Nothing Then
                Try
                    ComputerEntry.MoveTo(OUEntry)
                    ComputerEntry.CommitChanges()
                    ComputerEntry.RefreshCache()
                    logger.Debug("Moved object ""{0}"" to Destination ""{1}""", ComputerEntry.Name, ComputerEntry.Parent.Path)
                    Result = True
                Catch exc As Exception
                    logger.DebugException(String.Format("Exception: {0}.", exc.Message), exc)
                End Try

                'Clean up
                ComputerEntry.Close()
                ComputerEntry.Dispose()
                OUEntry.Dispose()
            Else
                logger.Debug("Unable to get Directory Entries for computer and OU. Skipping further processing.")
            End If
        Else
            logger.Error("Computer name or OU path not supplied. Skipping further processing")
        End If

        If Result Then
            logger.Info("successfuly moved computer {0} to new OU {1}. Returning ""True""", ComputerName, OUPath)
        Else
            logger.Info("Unable to move computer {0} to new OU {1}. Returning ""False""", ComputerName, OUPath)
        End If

        Return Result
    End Function
#End Region


#Region "Helper Functions"
    ''' <summary>
    ''' Returns the the LDAP path of the current Domain
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetRootDSE() As String
        Dim rootDse = New DirectoryEntry("LDAP://RootDSE")
        rootDse.AuthenticationType = AuthenticationTypes.Secure

        Dim propertyValue As Object = rootDse.Properties("defaultNamingContext").Value

        Return If(propertyValue IsNot Nothing, propertyValue.ToString(), Nothing)
    End Function

    ''' <summary>
    ''' Add "LDAP://" to the supplied path if necessary
    ''' </summary>
    ''' <param name="Path">Path to Directory object</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function AddLDAP(ByVal Path As String) As String
        Dim Result As String = Path

        If Not Path.StartsWith("LDAP://") Then
            Result = "LDAP://" + Path
        End If

        Return Result
    End Function
#End Region
End Class