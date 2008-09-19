<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.ReconcileQueue.ReconcileQueueItemList"
	Codebehind="ReconcileQueueItemList.ascx.cs" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="ReconcileQueueDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.ReconcileQueueDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.ReconcileQueueSummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetReconcileQueueDataSource"
				OnObjectDisposing="DisposeReconcileQueueDataSource"/>
				<ccUI:GridView ID="ReconcileQueueGridView" runat="server" SkinID="CustomGlobalGridView"
					OnSelectedIndexChanged="ReconcileQueueGridView_SelectedIndexChanged"
					OnPageIndexChanging="ReconcileQueueGridView_PageIndexChanging"
					SelectionMode="Single" DataSourceID="ReconcileQueueDataSourceObject">
					<Columns>
					    <asp:BoundField DataField="StudyInstanceUID" HeaderText="Study Instance UID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:BoundField DataField="ExistingPatientName" HeaderText="Existing Patient" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</asp:BoundField>
						<asp:BoundField DataField="ConflictingPatientName" HeaderText="Conflicting Patient" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="red">
						</asp:BoundField>
						<asp:BoundField DataField="ReceivedTime" HeaderText="Time Received" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</asp:BoundField>
					</Columns>
					<EmptyDataTemplate>
						<asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
							CssClass="GlobalGridViewHeader">
							<asp:TableHeaderRow Height="23px">
								<asp:TableHeaderCell HorizontalAlign="Left">Study Instance UID</asp:TableHeaderCell>
								<asp:TableHeaderCell HorizontalAlign="Left">Existing Patient</asp:TableHeaderCell>
								<asp:TableHeaderCell HorizontalAlign="Left">Conflicting Patient</asp:TableHeaderCell>
								<asp:TableHeaderCell HorizontalAlign="Center">Time Received</asp:TableHeaderCell>
							</asp:TableHeaderRow>
                        </asp:Table>							
					</EmptyDataTemplate>
					<RowStyle CssClass="GlobalGridViewRow" />
					<AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
					<SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
					<HeaderStyle CssClass="GlobalGridViewHeader" />
					<PagerTemplate>
					</PagerTemplate>
				</ccUI:GridView>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
