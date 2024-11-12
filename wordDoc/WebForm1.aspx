<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebForm1.aspx.cs" Inherits="YourNamespace.WebForm1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>IME Report Entry</title>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        function downloadReport(claimantId) {
            event.preventDefault();
            $.ajax({
                type: "POST",
                url: "https://localhost:44342/WebService1.asmx/GenerateIMEReport", // Update with your actual WebService URL
                data: JSON.stringify({ claimantId: claimantId }),
                contentType: "application/json; charset=utf-8",
                xhrFields: {
                    responseType: 'blob'
                },
                success: function (response, status, xhr) {
                    var blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' });

                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = 'IME_Report.docx'; // Desired file name

                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                },
                error: function (xhr, status, error) {
                    console.log('Error downloading report:', xhr.status, error);
                    alert('Failed to download the report. Status: ' + xhr.status);
                }
            });
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>IME Report Data</h2

            <!-- Table to display data -->
           <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                          OnRowEditing="GridView1_RowEditing" 
                          OnRowDeleting="GridView1_RowDeleting" 
                          OnRowUpdating="GridView1_RowUpdating" 
                          DataKeyNames="ClaimantID">
                <Columns>
                    <asp:BoundField DataField="ClaimantID" HeaderText="Claimant ID" ReadOnly="True" />

                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Date of Birth">
                        <ItemTemplate>
                            <asp:Label ID="lblDateOfBirth" runat="server" Text='<%# Eval("DateOfBirth", "{0:yyyy-MM-dd}") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDateOfBirth" runat="server" Text='<%# Bind("DateOfBirth", "{0:yyyy-MM-dd}") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Age">
                        <ItemTemplate>
                            <asp:Label ID="lblAge" runat="server" Text='<%# Eval("Age") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtAge" runat="server" Text='<%# Bind("Age") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Address">
                        <ItemTemplate>
                            <asp:Label ID="lblAddress" runat="server" Text='<%# Eval("Address") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtAddress" runat="server" Text='<%# Bind("Address") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Occupation">
                        <ItemTemplate>
                            <asp:Label ID="lblOccupation" runat="server" Text='<%# Eval("Occupation") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtOccupation" runat="server" Text='<%# Bind("Occupation") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:CommandField ShowEditButton="True" />
                    <asp:CommandField ShowDeleteButton="True" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnDownload" runat="server" Text="Download" 
                                        CommandArgument='<%# Eval("ClaimantID") %>' 
                                        OnClientClick="downloadReport(this.getAttribute('data-claimant-id')); return false;" 
                                        data-claimant-id='<%# Eval("ClaimantID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>


            <!-- Form for Adding Information -->
             <h2>Claimant Information</h2>
            <label>Name:</label>
            <asp:TextBox ID="txtName" runat="server"></asp:TextBox><br />
            <label>Date of Birth:</label>
            <asp:TextBox ID="txtDateOfBirth" runat="server" TextMode="Date"></asp:TextBox><br />
            <label>Age:</label>
            <asp:TextBox ID="txtAge" runat="server"></asp:TextBox><br />
            <label>Address:</label>
            <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox><br />
            <label>Occupation:</label>
            <asp:TextBox ID="txtOccupation" runat="server"></asp:TextBox><br />
            <label>Onset of Illness:</label>
            <asp:TextBox ID="txtOnsetOfIllness" runat="server" TextMode="Date"></asp:TextBox><br />
            <label>Work Cessation Date:</label>
            <asp:TextBox ID="txtWorkCessationDate" runat="server" TextMode="Date"></asp:TextBox><br />
            <label>Work Resumption Date:</label>
            <asp:TextBox ID="txtWorkResumptionDate" runat="server" TextMode="Date"></asp:TextBox><br />

            <h2>Assessment</h2>
            <label>Assessment Date:</label>
            <asp:TextBox ID="txtAssessmentDate" runat="server" TextMode="Date"></asp:TextBox><br />
            <label>Assessment Duration:</label>
            <asp:TextBox ID="txtAssessmentDuration" runat="server"></asp:TextBox><br />
            <label>Examination Place:</label>
            <asp:TextBox ID="txtExaminationPlace" runat="server"></asp:TextBox><br />
            <label>Requested Copy:</label>
            <asp:CheckBox ID="chkRequestedCopy" runat="server" /><br />
            <label>Date Sent:</label>
            <asp:TextBox ID="txtDateSent" runat="server" TextMode="Date"></asp:TextBox><br />
            <label>Permission Release:</label>
            <asp:CheckBox ID="chkPermissionRelease" runat="server" /><br />

            <h2>History</h2>
            <label>Occupation Details:</label>
            <asp:TextBox ID="txtOccupationDetails" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Illness Onset Details:</label>
            <asp:TextBox ID="txtIllnessOnsetDetails" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Initial Treatment:</label>
            <asp:TextBox ID="txtInitialTreatment" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Subsequent Progress:</label>
            <asp:TextBox ID="txtSubsequentProgress" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Current Status:</label>
            <asp:TextBox ID="txtCurrentStatus" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Work Status:</label>
            <asp:TextBox ID="txtWorkStatus" runat="server"></asp:TextBox><br />
            <label>Present Activities:</label>
            <asp:TextBox ID="txtPresentActivities" runat="server"></asp:TextBox><br />

            <h2>Medical History</h2>
            <label>Past Medical History:</label>
            <asp:TextBox ID="txtPastMedicalHistory" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Family History:</label>
            <asp:TextBox ID="txtFamilyHistory" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Personal Social History:</label>
            <asp:TextBox ID="txtPersonalSocialHistory" runat="server" TextMode="MultiLine"></asp:TextBox><br />

            <h2>Examination</h2>
            <label>Physical Examination:</label>
            <asp:TextBox ID="txtPhysicalExamination" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Mental Health Evaluation:</label>
            <asp:TextBox ID="txtMentalHealthEvaluation" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            
            <h2>Summary and Assessment</h2>
            <label>Summary:</label>
            <asp:TextBox ID="txtSummary" runat="server" TextMode="MultiLine"></asp:TextBox><br />
            <label>Remarks:</label>
            <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine"></asp:TextBox><br />

            <asp:Button ID="btnAdd" runat="server" Text="Add New" OnClick="btnAdd_Click" />
        </div>
    </form>
</body>
</html>
