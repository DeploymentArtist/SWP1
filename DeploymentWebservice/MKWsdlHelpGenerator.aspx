<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MKWsdlHelpGenerator.aspx.vb"
    Inherits="MaikKoster.Deployment.WebService.MKWsdlHelpGenerator" %>

<%@ Import Namespace="System.Web.Services.Description" %>
<html>
<head>
    <link rel="alternate" type="text/xml" href="<%#FileName%>?disco" />
    <link href="WebServiceDocumentation.css" type="text/css" rel="stylesheet" />
    <title>
        <%#ServiceName & " " & GetLocalizedText("WebService")%></title>
</head>
<body>
    <div id="content">
        <p class="heading1">
            <%#ServiceName%></p>
        <br />
        <span id="Span1" visible='<%#ShowingMethodList AndAlso ServiceDocumentation.Length > 0%>'
            runat="server">
            <p class="intro">
                <%#ServiceDocumentation%></p>
        </span><span id="Span2" visible='<%#ShowingMethodList%>' runat="server">
            <p class="intro">
                <%#GetLocalizedText("OperationsIntro", New Object() {EscapedFileName + "?WSDL"})%></p>
            <asp:Repeater ID="MethodList" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><a href="<%#EscapedFileName%>?op=<%#EscapeParam(DataBinder.Eval(Container.DataItem, "Key").ToString())%>">
                        <%#DataBinder.Eval(Container.DataItem, "Key")%></a> <span id="Span3" visible='<%#DirectCast(DataBinder.Eval(Container.DataItem, "Value.Documentation"), String).Length > 0%>'
                            runat="server">
                            <br>
                            
                            <%#DataBinder.Eval(Container.DataItem, "Value.Documentation")%>
                        </span>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </span><span id="Span4" visible='<%#Not ShowingMethodList AndAlso OperationExists%>' runat="server">
            <p class="intro">
                <%#GetLocalizedText("LinkBack", New Object() {EscapedFileName})%></p>
            <h2>
                <%#OperationName%></h2>
            <p class="intro">
                <%= Iif(SoapOperationBinding Is Nothing, "", SoapOperation.Documentation)%></p>
            <h3>
                <%#GetLocalizedText("TestHeader")%></h3>
            <% If Not showPost Then
                    If Not ShowingHttpGet Then%>
            <%#GetLocalizedText("NoHttpGetTest")%>
            <% 
            Else
                If Not ShowGetTestForm Then%>
            <%#GetLocalizedText("NoTestNonPrimitive")%>
            <% 
            Else%>
            <%#GetLocalizedText("TestText")%>
            <form target="_blank" action='<% If Not TryGetURL Is Nothing Then Response.Write(TryGetURL.AbsoluteUri)%>' method="get">
            <asp:Repeater DataSource='<%#TryGetMessageParts%>' runat="server">
                <HeaderTemplate>
                    <table cellspacing="0" cellpadding="4" frame="box" bordercolor="#dcdcdc" rules="none"
                        style="border-collapse: collapse;">
                        <tr id="Tr1" visible='<%# TryGetMessageParts.Length > 0%>' runat="server">
                            <td class="frmHeader" background="#dcdcdc" style="border-right: 2px solid white;">
                                <%#GetLocalizedText("Parameter")%>
                            </td>
                            <td class="frmHeader" background="#dcdcdc">
                                <%#GetLocalizedText("Value")%>
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="frmText" style="color: #000000; font-weight: normal;">
                            <%# DirectCast(Container.DataItem, MessagePart).Name%>:
                        </td>
                        <td>
                            <input class="frmInput" type="text" size="50" name="<%# DirectCast(Container.DataItem, MessagePart).Name %>">
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            <input type="submit" value="<%#GetLocalizedText("InvokeButton")%>" class="button">
                        </td>
                    </tr>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            </form>
            <% End If
            End If
             
        Else  'showPost
            If Not ShowingHttpPost Then
                If requestIsLocal Then%>
            <%#GetLocalizedText("NoTestNonPrimitive")%>
            <% 
            Else%>
            <%#GetLocalizedText("NoTestFormRemote")%>
            <% 
            End If
        Else
            If Not ShowPostTestForm Then%>
            <%#GetLocalizedText("NoTestNonPrimitive")%>
            <% 
            Else%>
            <%#GetLocalizedText("TestText")%>
            <form target="_blank" action='<% If Not TryGetURL Is Nothing Then Response.Write(TryGetURL.AbsoluteUri)%>' method="post">
            <asp:Repeater DataSource='<%#TryPostMessageParts%>' runat="server">
                <HeaderTemplate>
                    <table cellspacing="0" cellpadding="4" frame="box" bordercolor="#dcdcdc" rules="none"
                        style="border-collapse: collapse;">
                        <tr id="Tr2" visible='<%# TryPostMessageParts.Length > 0%>' runat="server">
                            <td class="frmHeader" background="#dcdcdc" style="border-right: 2px solid white;">
                                <%#GetLocalizedText("Parameter")%>
                            </td>
                            <td class="frmHeader" background="#dcdcdc">
                                <%#GetLocalizedText("Value")%>
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="frmText" style="color: #000000; font-weight: normal;">
                            <%# DirectCast(Container.DataItem, MessagePart).Name%>:
                        </td>
                        <td>
                            <input class="frmInput" type="text" size="50" name="<%# DirectCast(Container.DataItem, MessagePart).Name %>">
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            <input type="submit" value="<%#GetLocalizedText("InvokeButton")%>" class="button">
                        </td>
                    </tr>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            </form>
            <% End If
            End If
        End If%>
            <span id="Span5" visible='<%#ShowingSoap%>' runat="server">
                <h3>
                    <%#GetLocalizedText("SoapTitle")%></h3>
                <p>
                    <%#GetLocalizedText("SoapText")%></p>
                <pre><%#SoapOperationInput%></pre>
                <pre><%#SoapOperationOutput%></pre>
            </span><span id="Span6" visible='<%#ShowingHttpGet%>' runat="server">
                <h3>
                    <%#GetLocalizedText("HttpGetTitle")%></h3>
                <p>
                    <%#GetLocalizedText("HttpGetText")%></p>
                <pre><%#HttpGetOperationInput%></pre>
                <pre><%#HttpGetOperationOutput%></pre>
            </span><span id="Span7" visible='<%#ShowingHttpPost%>' runat="server">
                <h3>
                    <%#GetLocalizedText("HttpPostTitle")%></h3>
                <p>
                    <%#GetLocalizedText("HttpPostText")%></p>
                <pre><%#HttpPostOperationInput%></pre>
                <pre><%#HttpPostOperationOutput%></pre>
            </span></span><span id="Span8" visible='<%#ShowingMethodList AndAlso ServiceNamespace = "http://tempuri.org/"%>'
                runat="server">
                <hr>
                <h3>
                    <%#GetLocalizedText("DefaultNamespaceWarning1")%></h3>
                <h3>
                    <%#GetLocalizedText("DefaultNamespaceWarning2")%></h3>
                <p class="intro">
                    <%#GetLocalizedText("DefaultNamespaceHelp1")%></p>
                <p class="intro">
                    <%#GetLocalizedText("DefaultNamespaceHelp2")%></p>
                <p class="intro">
                    <%#GetLocalizedText("DefaultNamespaceHelp3")%></p>
                <p class="intro">
                    C#</p>
                <pre>[WebService(Namespace="http://microsoft.com/webservices/")]
public class MyWebService {
    // <%#GetLocalizedText("Implementation")%>
}</pre>
                <p class="intro">
                    Visual Basic.NET</p>
                <pre>&lt;WebService(Namespace:="http://microsoft.com/webservices/")&gt; Public Class MyWebService
    ' <%#GetLocalizedText("Implementation")%>
End Class</pre>
                <p class="intro">
                    <%#GetLocalizedText("DefaultNamespaceHelp4")%></p>
                <p class="intro">
                    <%#GetLocalizedText("DefaultNamespaceHelp5")%></p>
                <p class="intro">
                    <%#GetLocalizedText("DefaultNamespaceHelp6")%></p>
            </span><span id="Span9" visible='<%#Not ShowingMethodList AndAlso Not OperationExists%>' runat="server">
                <%#GetLocalizedText("LinkBack", New Object() {EscapedFileName})%>
                <h2>
                    <%#GetLocalizedText("MethodNotFound")%></h2>
                <%#GetLocalizedText("MethodNotFoundText", New Object() {Server.HtmlEncode(OperationName), ServiceName})%>
            </span>
    </div>
</body>
</html>
