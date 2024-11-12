using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Xml.Linq;

namespace YourNamespace
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionStrings"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Claimant";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void btnCreateNew_Click(object sender, EventArgs e)
        {
            // Clear all fields for new entry
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtDateOfBirth.Text = "";
            txtAge.Text = "";
            txtAddress.Text = "";
            txtOccupation.Text = "";
            txtOnsetOfIllness.Text = "";
            txtWorkCessationDate.Text = "";
            txtWorkResumptionDate.Text = "";
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Insert into Claimant table
                string queryClaimant = "INSERT INTO Claimant (Name, DateOfBirth, Age, Address, Occupation, OnsetOfIllness, WorkCessationDate, WorkResumptionDate) " +
                                       "VALUES (@Name, @DateOfBirth, @Age, @Address, @Occupation, @OnsetOfIllness, @WorkCessationDate, @WorkResumptionDate); " +
                                       "SELECT SCOPE_IDENTITY();";
                SqlCommand cmdClaimant = new SqlCommand(queryClaimant, conn);
                cmdClaimant.Parameters.AddWithValue("@Name", txtName.Text);
                cmdClaimant.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(txtDateOfBirth.Text));
                cmdClaimant.Parameters.AddWithValue("@Age", txtAge.Text);
                cmdClaimant.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmdClaimant.Parameters.AddWithValue("@Occupation", txtOccupation.Text);
                cmdClaimant.Parameters.AddWithValue("@OnsetOfIllness", DateTime.Parse( txtOnsetOfIllness.Text));
                cmdClaimant.Parameters.AddWithValue("@WorkCessationDate", DateTime.Parse( txtWorkCessationDate.Text));
                cmdClaimant.Parameters.AddWithValue("@WorkResumptionDate", DateTime.Parse(txtWorkResumptionDate.Text));

                int claimantID = Convert.ToInt32(cmdClaimant.ExecuteScalar());

                // Insert into Assessment table
                string queryAssessment = "INSERT INTO Assessment (ClaimantID, AssessmentDate, AssessmentDuration, ExaminationPlace, RequestedCopy, DateSent, PermissionRelease) " +
                                         "VALUES (@ClaimantID, @AssessmentDate, @AssessmentDuration, @ExaminationPlace, @RequestedCopy, @DateSent, @PermissionRelease)";
                SqlCommand cmdAssessment = new SqlCommand(queryAssessment, conn);
                cmdAssessment.Parameters.AddWithValue("@ClaimantID", claimantID);
                cmdAssessment.Parameters.AddWithValue("@AssessmentDate", DateTime.Parse( txtAssessmentDate.Text));
                cmdAssessment.Parameters.AddWithValue("@AssessmentDuration", txtAssessmentDuration.Text);
                cmdAssessment.Parameters.AddWithValue("@ExaminationPlace", txtExaminationPlace.Text);
                cmdAssessment.Parameters.AddWithValue("@RequestedCopy", chkRequestedCopy.Checked);
                cmdAssessment.Parameters.AddWithValue("@DateSent", DateTime.Parse(txtDateSent.Text));
                cmdAssessment.Parameters.AddWithValue("@PermissionRelease", chkPermissionRelease.Checked);
                cmdAssessment.ExecuteNonQuery();

                // Insert into History table
                string queryHistory = "INSERT INTO History (ClaimantID, OccupationDetails, IllnessOnsetDetails, InitialTreatment, SubsequentProgress, CurrentStatus, WorkStatus, PresentActivities) " +
                                      "VALUES (@ClaimantID, @OccupationDetails, @IllnessOnsetDetails, @InitialTreatment, @SubsequentProgress, @CurrentStatus, @WorkStatus, @PresentActivities)";
                SqlCommand cmdHistory = new SqlCommand(queryHistory, conn);
                cmdHistory.Parameters.AddWithValue("@ClaimantID", claimantID);
                cmdHistory.Parameters.AddWithValue("@OccupationDetails", txtOccupationDetails.Text);
                cmdHistory.Parameters.AddWithValue("@IllnessOnsetDetails", txtIllnessOnsetDetails.Text);
                cmdHistory.Parameters.AddWithValue("@InitialTreatment", txtInitialTreatment.Text);
                cmdHistory.Parameters.AddWithValue("@SubsequentProgress", txtSubsequentProgress.Text);
                cmdHistory.Parameters.AddWithValue("@CurrentStatus", txtCurrentStatus.Text);
                cmdHistory.Parameters.AddWithValue("@WorkStatus", txtWorkStatus.Text);
                cmdHistory.Parameters.AddWithValue("@PresentActivities", txtPresentActivities.Text);
                cmdHistory.ExecuteNonQuery();

                // Insert into Medical table
                string queryMedical = "INSERT INTO Medical (ClaimantID, PastMedicalHistory, FamilyHistory, PersonalSocialHistory) " +
                                      "VALUES (@ClaimantID, @PastMedicalHistory, @FamilyHistory, @PersonalSocialHistory)";
                SqlCommand cmdMedical = new SqlCommand(queryMedical, conn);
                cmdMedical.Parameters.AddWithValue("@ClaimantID", claimantID);
                cmdMedical.Parameters.AddWithValue("@PastMedicalHistory", txtPastMedicalHistory.Text);
                cmdMedical.Parameters.AddWithValue("@FamilyHistory", txtFamilyHistory.Text);
                cmdMedical.Parameters.AddWithValue("@PersonalSocialHistory", txtPersonalSocialHistory.Text);
                cmdMedical.ExecuteNonQuery();

                // Insert into Examination table
                string queryExamination = "INSERT INTO Examination (ClaimantID, PhysicalDetails, PsychologicalDetails) " +
                                          "VALUES (@ClaimantID, @PhysicalDetails, @PsychologicalDetails)";
                SqlCommand cmdExamination = new SqlCommand(queryExamination, conn);
                cmdExamination.Parameters.AddWithValue("@ClaimantID", claimantID);
                cmdExamination.Parameters.AddWithValue("@PhysicalDetails", txtPhysicalExamination.Text);
                cmdExamination.Parameters.AddWithValue("@PsychologicalDetails", txtMentalHealthEvaluation.Text);
                cmdExamination.ExecuteNonQuery();

                // Insert into Summary table
                string querySummary = "INSERT INTO Summary (ClaimantID, Diagnosis, ClinicalOpinion, StandardQuestions, AdditionalQuestions) " +
                                      "VALUES (@ClaimantID, @Diagnosis, @ClinicalOpinion, @StandardQuestions, @AdditionalQuestions)";
                SqlCommand cmdSummary = new SqlCommand(querySummary, conn);
                cmdSummary.Parameters.AddWithValue("@ClaimantID", claimantID);
                cmdSummary.Parameters.AddWithValue("@Diagnosis", txtSummary.Text); // Assuming Summary as Diagnosis
                cmdSummary.Parameters.AddWithValue("@ClinicalOpinion", txtRemarks.Text);
                cmdSummary.Parameters.AddWithValue("@StandardQuestions", DBNull.Value); // Adjust if needed
                cmdSummary.Parameters.AddWithValue("@AdditionalQuestions", DBNull.Value); // Adjust if needed
                cmdSummary.ExecuteNonQuery();

                conn.Close();
            }

            BindGrid();
        }


        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int claimantId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            GridViewRow row = GridView1.Rows[e.RowIndex];

            System.Diagnostics.Debug.WriteLine("hi");

            // Fetch new values from the form controls
            string name = (row.FindControl("txtName") as TextBox)?.Text;
            string dateOfBirth = (row.FindControl("txtDateOfBirth") as TextBox)?.Text;
            string ageText = (row.FindControl("txtAge") as TextBox)?.Text;

            // Get current values from the database for comparison
            string currentName = GetCurrentValueFromDatabase(claimantId, "Name");
            string currentDateOfBirth = GetCurrentValueFromDatabase(claimantId, "DateOfBirth");
            string currentAge = GetCurrentValueFromDatabase(claimantId, "Age");



            // Check if the new value is different from the current value
            bool isNameChanged = false;
            if (!string.IsNullOrEmpty(name) && name != currentName) { isNameChanged = true; }

            bool isDateOfBirthChanged = false;
            if (!string.IsNullOrEmpty(dateOfBirth) && dateOfBirth != currentDateOfBirth) { isDateOfBirthChanged = true; }

            bool isAgeChanged = false;
            if(ageText != string.Empty && ageText != currentAge) { isAgeChanged = true; }

            System.Diagnostics.Debug.WriteLine(name);

            System.Diagnostics.Debug.WriteLine(currentName);

            System.Diagnostics.Debug.WriteLine("hi2");
            // Only construct the update query if at least one field has changed
            if (isNameChanged || isDateOfBirthChanged || isAgeChanged)
            {

                System.Diagnostics.Debug.WriteLine("hi3");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    System.Diagnostics.Debug.WriteLine("hi4");

                    // Construct the query dynamically based on the changed fields
                    string query = "UPDATE Claimant SET ";
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (isNameChanged)
                    {
                        query += "Name=@Name, ";
                        parameters.Add(new SqlParameter("@Name", name ?? (object)DBNull.Value)); // Use DBNull.Value for null values
                    }

                    if (isDateOfBirthChanged)
                    {
                        query += "DateOfBirth=@DateOfBirth, ";
                        parameters.Add(new SqlParameter("@DateOfBirth", dateOfBirth ?? (object)DBNull.Value)); // Use DBNull.Value for null values
                    }

                    if (isAgeChanged)
                    {
                        query += "Age=@Age ";
                        parameters.Add(new SqlParameter("@Age", ageText ?? (object)DBNull.Value)); // Use DBNull.Value for null values
                    }

                    // Remove the last comma if any field was updated
                    if (query.EndsWith(", "))
                    {
                        query = query.Substring(0, query.Length - 2);
                    }

                    query += " WHERE ClaimantID=@ClaimantID";

                    // Add the ClaimantID parameter
                    parameters.Add(new SqlParameter("@ClaimantID", claimantId));
                    System.Diagnostics.Debug.WriteLine(query);

                    // Execute the query
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddRange(parameters.ToArray());
                    cmd.ExecuteNonQuery();
                }
            }

            // Reset editing index and refresh grid
            GridView1.EditIndex = -1;
            BindGrid();
        }

        // Method to get current value from database (replace with actual data retrieval)
        private string GetCurrentValueFromDatabase(int claimantId, string columnName)
        {
            string value = null;
            string query = $"SELECT {columnName} FROM Claimant WHERE ClaimantID = @ClaimantID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClaimantID", claimantId);
                value = cmd.ExecuteScalar() as string;
            }
            return value;
        }


        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int claimantId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Start a transaction to ensure all deletions are processed together
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Delete from related tables first
                    string deleteAssessment = "DELETE FROM Assessment WHERE ClaimantID=@ClaimantID";
                    SqlCommand cmdAssessment = new SqlCommand(deleteAssessment, conn, transaction);
                    cmdAssessment.Parameters.AddWithValue("@ClaimantID", claimantId);
                    cmdAssessment.ExecuteNonQuery();

                    string deleteHistory = "DELETE FROM History WHERE ClaimantID=@ClaimantID";
                    SqlCommand cmdHistory = new SqlCommand(deleteHistory, conn, transaction);
                    cmdHistory.Parameters.AddWithValue("@ClaimantID", claimantId);
                    cmdHistory.ExecuteNonQuery();

                    string deleteMedical = "DELETE FROM Medical WHERE ClaimantID=@ClaimantID";
                    SqlCommand cmdMedical = new SqlCommand(deleteMedical, conn, transaction);
                    cmdMedical.Parameters.AddWithValue("@ClaimantID", claimantId);
                    cmdMedical.ExecuteNonQuery();

                    string deleteExamination = "DELETE FROM Examination WHERE ClaimantID=@ClaimantID";
                    SqlCommand cmdExamination = new SqlCommand(deleteExamination, conn, transaction);
                    cmdExamination.Parameters.AddWithValue("@ClaimantID", claimantId);
                    cmdExamination.ExecuteNonQuery();

                    string deleteSummary = "DELETE FROM Summary WHERE ClaimantID=@ClaimantID";
                    SqlCommand cmdSummary = new SqlCommand(deleteSummary, conn, transaction);
                    cmdSummary.Parameters.AddWithValue("@ClaimantID", claimantId);
                    cmdSummary.ExecuteNonQuery();

                    // Finally, delete from the main Claimant table
                    string deleteClaimant = "DELETE FROM Claimant WHERE ClaimantID=@ClaimantID";
                    SqlCommand cmdClaimant = new SqlCommand(deleteClaimant, conn, transaction);
                    cmdClaimant.Parameters.AddWithValue("@ClaimantID", claimantId);
                    cmdClaimant.ExecuteNonQuery();

                    // Commit the transaction if all deletions are successful
                    transaction.Commit();
                }
                catch
                {
                    // Roll back the transaction in case of an error
                    transaction.Rollback();
                    throw;
                }
            }

            // Refresh the grid
            BindGrid();
        }


        protected void btnDownload_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int claimantId = Convert.ToInt32(btn.CommandArgument);

            // Implement Word document download functionality here
            // For now, you can leave this empty or add a placeholder comment.
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Placeholder if additional save functionality is needed

        }
    }

}

