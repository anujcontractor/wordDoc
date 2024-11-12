using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace wordDoc
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://yournamespace.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public byte[] GenerateIMEReport(int claimantId)
        {
            // Fetch data from the database based on ClaimantID
            var data = GetClaimantData(claimantId);

            string templatePath = @"C:\Users\Dell\Downloads\IME Report Template Guide 2.0.docx";
            string outputPath = @"C:\\Users\\Dell\\Downloads\\GeneratedReport.docx";

            // Create a copy of the template
            File.Copy(templatePath, outputPath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(outputPath, true))
            {
                Body body = wordDoc.MainDocumentPart.Document.Body;

                // Claimant Table

                ReplacePlaceholder(body, "##ID##", claimantId.ToString());
                ReplacePlaceholder(body, "##Name##", data.Name);
                ReplacePlaceholder(body, "##Age##", data.Age.ToString());
                ReplacePlaceholder(body, "##DateOfBirth##", data.DateOfBirth.ToShortDateString());
                ReplacePlaceholder(body, "##Address##", data.Address);
                ReplacePlaceholder(body, "##Occupation##", data.Occupation);
                ReplacePlaceholder(body, "##OnsetOfIllness##", data.OnsetOfIllness?.ToShortDateString() ?? "N/A");
                ReplacePlaceholder(body, "##WorkCessationDate##", data.WorkCessationDate?.ToShortDateString() ?? "N/A");
                ReplacePlaceholder(body, "##WorkResumptionDate##", data.WorkResumptionDate?.ToShortDateString() ?? "N/A");

                // Assessment Table
                ReplacePlaceholder(body, "##AssessmentDate##", data.AssessmentDate?.ToShortDateString() ?? "N/A");
                ReplacePlaceholder(body, "##AssessmentDuration##", data.AssessmentDuration);
                ReplacePlaceholder(body, "##ExaminationPlace##", data.ExaminationPlace);
                ReplacePlaceholder(body, "##RequestedCopy##", data.RequestedCopy.HasValue ? (data.RequestedCopy.Value ? "Yes" : "No") : "N/A");
                ReplacePlaceholder(body, "##DateSent##", data.DateSent?.ToShortDateString() ?? "N/A");
                ReplacePlaceholder(body, "##PermissionRelease##", data.PermissionRelease.HasValue ? (data.PermissionRelease.Value ? "Yes" : "No") : "N/A");

                // History Table
                ReplacePlaceholder(body, "##OccupationDetails##", data.OccupationDetails);
                ReplacePlaceholder(body, "##IllnessOnsetDetails##", data.IllnessOnsetDetails);
                ReplacePlaceholder(body, "##InitialTreatment##", data.InitialTreatment);
                ReplacePlaceholder(body, "##SubsequentProgress##", data.SubsequentProgress);
                ReplacePlaceholder(body, "##CurrentStatus##", data.CurrentStatus);
                ReplacePlaceholder(body, "##WorkStatus##", data.WorkStatus);
                ReplacePlaceholder(body, "##PresentActivities##", data.PresentActivities);

                // Medical Table
                ReplacePlaceholder(body, "##PastMedicalHistory##", data.PastMedicalHistory);
                ReplacePlaceholder(body, "##FamilyHistory##", data.FamilyHistory);
                ReplacePlaceholder(body, "##PersonalSocialHistory##", data.PersonalSocialHistory);

                // Examination Table
                ReplacePlaceholder(body, "##PhysicalDetails##", data.PhysicalDetails);
                ReplacePlaceholder(body, "##PsychologicalDetails##", data.PsychologicalDetails);

                // Summary Table
                ReplacePlaceholder(body, "##Diagnosis##", data.Diagnosis);
                ReplacePlaceholder(body, "##ClinicalOpinion##", data.ClinicalOpinion);
                ReplacePlaceholder(body, "##StandardQuestions##", data.StandardQuestions);
                ReplacePlaceholder(body, "##AdditionalQuestions##", data.AdditionalQuestions);


                wordDoc.MainDocumentPart.Document.Save();
            }

            // Return the document as a byte array
            return File.ReadAllBytes(outputPath);
        }

        private ClaimantData GetClaimantData(int claimantId)
        {
            // Connect to the database and fetch data for the claimant
            var data = new ClaimantData();

            using (SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mydatabase;Integrated Security=True;Encrypt=False;MultipleActiveResultSets=True"))
            {
                conn.Open();
                string query = "SELECT c.ClaimantID, c.Name, c.DateOfBirth, c.Age, c.Address, c.Occupation, c.OnsetOfIllness, c.WorkCessationDate, c.WorkResumptionDate," +
                    " a.AssessmentID, a.AssessmentDate, a.AssessmentDuration, a.ExaminationPlace, a.RequestedCopy, a.DateSent, a.PermissionRelease," +
                    " h.HistoryID, h.OccupationDetails, h.IllnessOnsetDetails, h.InitialTreatment, h.SubsequentProgress, h.CurrentStatus, h.WorkStatus, h.PresentActivities," +
                    " m.MedicalID, m.PastMedicalHistory, m.FamilyHistory, m.PersonalSocialHistory," +
                    " e.ExaminationID, e.PhysicalDetails, e.PsychologicalDetails," +
                    " s.SummaryID, s.Diagnosis, s.ClinicalOpinion, s.StandardQuestions, s.AdditionalQuestions" +
                    " FROM Claimant c" +
                    " LEFT JOIN Assessment a ON c.ClaimantID = a.ClaimantID" +
                    " LEFT JOIN History h ON c.ClaimantID = h.ClaimantID" +
                    " LEFT JOIN Medical m ON c.ClaimantID = m.ClaimantID" +
                    " LEFT JOIN Examination e ON c.ClaimantID = e.ClaimantID" +
                    " LEFT JOIN Summary s ON c.ClaimantID = s.ClaimantID" +
                    " WHERE c.ClaimantID = @ClaimantID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClaimantID", claimantId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Claimant Table
                        data.Name = reader["Name"].ToString();
                        data.Age = Convert.ToInt32(reader["Age"]);
                        data.DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                        data.Address = reader["Address"].ToString();
                        data.WorkCessationDate = Convert.ToDateTime(reader["WorkCessationDate"]);
                        data.Occupation = reader["Occupation"].ToString() ;
                        data.OnsetOfIllness = Convert.ToDateTime(reader["OnsetOfIllness"]);
                        data.WorkResumptionDate = Convert.ToDateTime(reader["WorkResumptionDate"]);

                        // Assessment Table
                        data.AssessmentID = Convert.ToInt32(reader["AssessmentID"]);
                        data.AssessmentDate = reader["AssessmentDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["AssessmentDate"]);
                        data.AssessmentDuration = reader["AssessmentDuration"].ToString();
                        data.ExaminationPlace = reader["ExaminationPlace"].ToString();
                        data.RequestedCopy = reader["RequestedCopy"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(reader["RequestedCopy"]);
                        data.DateSent = reader["DateSent"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateSent"]);
                        data.PermissionRelease = reader["PermissionRelease"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(reader["PermissionRelease"]);

                        // History Table
                        data.HistoryID = Convert.ToInt32(reader["HistoryID"]);
                        data.OccupationDetails = reader["OccupationDetails"].ToString();
                        data.IllnessOnsetDetails = reader["IllnessOnsetDetails"].ToString();
                        data.InitialTreatment = reader["InitialTreatment"].ToString();
                        data.SubsequentProgress = reader["SubsequentProgress"].ToString();
                        data.CurrentStatus = reader["CurrentStatus"].ToString();
                        data.WorkStatus = reader["WorkStatus"].ToString();
                        data.PresentActivities = reader["PresentActivities"].ToString();

                        // Medical Table
                        data.MedicalID = Convert.ToInt32(reader["MedicalID"]);
                        data.PastMedicalHistory = reader["PastMedicalHistory"].ToString();
                        data.FamilyHistory = reader["FamilyHistory"].ToString();
                        data.PersonalSocialHistory = reader["PersonalSocialHistory"].ToString();

                        // Examination Table
                        data.ExaminationID = Convert.ToInt32(reader["ExaminationID"]);
                        data.PhysicalDetails = reader["PhysicalDetails"].ToString();
                        data.PsychologicalDetails = reader["PsychologicalDetails"].ToString();

                        // Summary Table
                        data.SummaryID = Convert.ToInt32(reader["SummaryID"]);
                        data.Diagnosis = reader["Diagnosis"].ToString();
                        data.ClinicalOpinion = reader["ClinicalOpinion"].ToString();
                        data.StandardQuestions = reader["StandardQuestions"].ToString();
                        data.AdditionalQuestions = reader["AdditionalQuestions"].ToString();
                    }
                }
            }

            return data;
        }

        private void ReplacePlaceholder(Body body, string placeholder, string value)
        {
            foreach (var text in body.Descendants<Text>())
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, value);
                }
            }
        }
    }

    public class ClaimantData
    {
        // Claimant Table
        public int ClaimantID { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Occupation { get; set; }
        public DateTime? OnsetOfIllness { get; set; }
        public DateTime? WorkCessationDate { get; set; }
        public DateTime? WorkResumptionDate { get; set; }

        // Assessment Table
        public int AssessmentID { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string AssessmentDuration { get; set; }
        public string ExaminationPlace { get; set; }
        public bool? RequestedCopy { get; set; }
        public DateTime? DateSent { get; set; }
        public bool? PermissionRelease { get; set; }

        // History Table
        public int HistoryID { get; set; }
        public string OccupationDetails { get; set; }
        public string IllnessOnsetDetails { get; set; }
        public string InitialTreatment { get; set; }
        public string SubsequentProgress { get; set; }
        public string CurrentStatus { get; set; }
        public string WorkStatus { get; set; }
        public string PresentActivities { get; set; }

        // Medical Table
        public int MedicalID { get; set; }
        public string PastMedicalHistory { get; set; }
        public string FamilyHistory { get; set; }
        public string PersonalSocialHistory { get; set; }

        // Examination Table
        public int ExaminationID { get; set; }
        public string PhysicalDetails { get; set; }
        public string PsychologicalDetails { get; set; }

        // Summary Table
        public int SummaryID { get; set; }
        public string Diagnosis { get; set; }
        public string ClinicalOpinion { get; set; }
        public string StandardQuestions { get; set; }
        public string AdditionalQuestions { get; set; }
    }

}
