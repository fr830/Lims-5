﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace ConsoleApp1
{
    public class DatabaseWriter
    {
        private static string connectionString =
        "Server=EALSQL1.eal.local; Database= DB2017_C08; User Id=USER_C08; Password=SesamLukOp_08";
      
        public void InsertSample(DatabaseAttribute da)
        {
            using (SqlConnection con = new SqlConnection(connectionString)) //will we need to close the connection manully if we exit the connection without execution the query??
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    if (da.SampleType == "ATAC-Seq") //change to switch/case
                    {
                        cmd = new SqlCommand("spAddSample_ATAC_Seq", con);
                    }
                    else if (da.SampleType == "ChIP-Seq")
                    {
                        cmd = new SqlCommand("spAddSample_ChIP_Seq", con);
                    }
                    else if (da.SampleType == "Hi-C")
                    {
                        cmd = new SqlCommand("spAddSample_Hi_C", con);
                    }
                    if (da.SampleType == "RNA-Seq")
                    {
                        cmd = new SqlCommand("spAddSample_RNA_Seq", con);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Sample_Type", da.SampleType));
                    cmd.Parameters.Add(new SqlParameter("@Genome_Type", da.GenomeType));
                    cmd.Parameters.Add(new SqlParameter("@Cell_Type", da.CellType));
                    cmd.Parameters.Add(new SqlParameter("@Treatment", da.Treatment));
                    cmd.Parameters.Add(new SqlParameter("@Condition", da.Condition));
                    cmd.Parameters.Add(new SqlParameter("@Comments", da.Comments));
                    cmd.Parameters.Add(new SqlParameter("@Concentration", da.Concentration));
                    cmd.Parameters.Add(new SqlParameter("@Volume", da.Volume));
                    cmd.Parameters.Add(new SqlParameter("@Initials", da.Initials));
                    cmd.Parameters.Add(new SqlParameter("@PI_Value", da.PIValue));
                    cmd.Parameters.Add(new SqlParameter("@Date_Of_Addition", da.DateOfAddition));

                    if (da.SampleType == "ATAC-Seq")
                    {
                        cmd.Parameters.Add(new SqlParameter("@Transposase_Unit", da.ATACTransposaseUnit));
                        cmd.Parameters.Add(new SqlParameter("@PCR_Cycles", da.ATACPCRCycles));
                    }
                    else if (da.SampleType == "ChIP-Seq")
                    {
                        cmd.Parameters.Add(new SqlParameter("@Antibody", da.ChIPAntibody));
                        cmd.Parameters.Add(new SqlParameter("@Antibody_Lot", da.ChIPAtibodyLot));
                        cmd.Parameters.Add(new SqlParameter("@Antibody_Catalogue_Number", da.ChIPAntibodyCatalogueNumber));
                    }
                    else if (da.SampleType == "Hi-C")
                    {
                        cmd.Parameters.Add(new SqlParameter("@Restriction_Enzyme", da.HIRestrictionEnzyme));
                        cmd.Parameters.Add(new SqlParameter("@PCR_Cycles", da.HIPCRCycles));
                    }
                    else if (da.SampleType == "RNA-Seq")
                    {
                        cmd.Parameters.Add(new SqlParameter("@Prep_Type", da.RNAPrepType));
                        cmd.Parameters.Add(new SqlParameter("@RIN", da.RNARIN));
                    }

                    cmd.ExecuteNonQuery();
                    con.Close();//needed? 
                }
                catch (SqlException e)
                {
                    MessageBox.Show(e.Message.ToString());
                }
            }
        }
    }
}
