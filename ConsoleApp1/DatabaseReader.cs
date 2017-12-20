﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace ConsoleApp1
{
    public class DatabaseReader
    {
        private static string connectionString =
                "Server=EALSQL1.eal.local; Database= DB2017_C08; User Id=USER_C08; Password=SesamLukOp_08";

        //Should we do this modularly by creating a method for each stored procedure
        //or should we have a method which get's the sampleID and sampleType from 
        //whatever type of value we search by ? it would seriously reduce redundancies
        //but it might break the single purpouse shit in SOLID, could argue that it is
        //a single purpose, as it only gets the sampleID and sampleType, just takes 
        //an array of different inputs...

        private List<string> SampleType = new List<string>();
        /*public void GetDBAccessValuesByString(string value, string spParameter)
        {                                           //put this in a seperate class ??
            string storedProcedure = string.Empty;
            switch (spParameter)
            {
                case "@Antibody":
                    storedProcedure = "spGetSampleByAntibody";
                    break;
                case "@Cell_Type":
                    storedProcedure = "spGetSampleByCellType";
                    break;
                case "@Condition":
                    storedProcedure = "spGetSampleByCondition";
                    break;
                case "@Initials":
                    storedProcedure = "spGetSampleByInitials";
                    break;
                case "@PI_Value":
                    storedProcedure = "spGetSampleByPI";
                    break;
                case "@Treatment":
                    storedProcedure = "spGetSampleByTreatment";
                    break;
            }
            GetSampleByStringValue(value, spParameter, storedProcedure);
        }*/
        /*public void GetSampleTypeByID(int sampleID)
        {
            string sampleType = string.Empty;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand GetSampleType = new SqlCommand("spGetSampleTypeByID", con);
                    GetSampleType.CommandType = CommandType.StoredProcedure;
                    GetSampleType.Parameters.Add(new SqlParameter("@Sample_ID", sampleID));

                    SqlDataReader reader = GetSampleType.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            sampleType = reader["Sample_Type"].ToString();
                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            GetSampleWithStoredProcedure(sampleID, sampleType);
        }
        */
        public List<string> GetSampleByParams(string value, string spParameter)
        {
            List<string> ret = new List<string>();
            string storedProcedure = GetStoredProcedureByParameter(value, spParameter);
            int[] sampleID = GetSampleIDWithStoredProcedure(value, spParameter, storedProcedure);
            string[] sampleType = SampleType.ToArray();
            for (int i = 0; i < sampleID.Length; i++)
            {
                ret.Add(GetSampleWithIDAndType(sampleID[i], SampleType[i]));
            }

            return ret;
        }
        private string GetStoredProcedureByParameter(string value, string spParameter)
        {
            string storedProcedure = string.Empty;
            switch (spParameter)
            {
                case "@Antibody":
                    storedProcedure = "spGetSampleByAntibody";
                    break;
                case "@Cell_Type":
                    storedProcedure = "spGetSampleByCellType";
                    break;
                case "@Condition":
                    storedProcedure = "spGetSampleByCondition";
                    break;
                case "@Initials":
                    storedProcedure = "spGetSampleByInitials";
                    break;
                case "@PI_Value":
                    storedProcedure = "spGetSampleByPI";
                    break;
                case "@Treatment":
                    storedProcedure = "spGetSampleByTreatment";
                    break;
            }
            return storedProcedure;
        }
        private int[] GetSampleIDWithStoredProcedure(string value, string spParameter, string storedProcedure)
        {
            List<int> sampleID = new List<int>();//initiate on top and clear ? dont need to access anywhere else
            SampleType.Clear();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand command = new SqlCommand(storedProcedure, con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter(spParameter, value));

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())//runs through this loop for any sample id relevant to the value looked for
                        {
                            if (!sampleID.Contains(int.Parse(reader["Sample_ID"].ToString())))
                            {
                                sampleID.Add(int.Parse(reader["Sample_ID"].ToString()));
                                SampleType.Add(reader["Sample_Type"].ToString());
                            }                            
                        }
                    }
                }
                catch (SqlException e)
                {
                    MessageBox.Show(e.Message.ToString());
                    //Console.WriteLine(e.Message);
                }
            }
            return sampleID.ToArray();
            //return sampleID;          can always return list instead if i think its useful..
        }

        private string GetSampleWithIDAndType(int sampleID, string sampleType)
        {
            string nl = "\n";
            string ret = string.Empty;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd1 = new SqlCommand("spGetSampleInfoFromIDAndType", con);
                    cmd1.CommandType = CommandType.StoredProcedure;

                    cmd1.Parameters.Add("@Sample_ID", SqlDbType.Int).Value = sampleID;
                    cmd1.Parameters.Add("@Sample_Type", SqlDbType.VarChar).Value = sampleType;

                    con.Open();
                    SqlDataReader reader = cmd1.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            //string xampleType = reader["Sample_Type"].ToString();//probably not needed as the sampletype is imported as an arg through the method parameters
                            string GenomeType = reader["Genome_Type"].ToString();
                            string CellType = reader["Cell_Type"].ToString();
                            string Treatment = reader["Treatment"].ToString();
                            string Condition = reader["Condition"].ToString();
                            string Comments = reader["Comments"].ToString();
                            string Concentration = reader["Concentration"].ToString();
                            string Volume = reader["Volume"].ToString();
                            string Initials = reader["Initials"].ToString();
                            string PiValue = reader["Pi_Value"].ToString();
                            string DateOfAddition = reader["Date_Of_Addition"].ToString();

                            ret = "SampleID:           " + sampleID + nl +
                                  "Sample Type:        " + sampleType + nl +
                                  "Cell Type:          " + CellType + nl +
                                  "Treatment:          " + Treatment + nl +
                                  "Condition:          " + Condition + nl +
                                  "Comments:           " + Comments + nl +
                                  "Concentration:      " + Concentration + nl +
                                  "Volume:             " + Volume + nl +
                                  "Initials:           " + Initials + nl +
                                  "PI Value:           " + PiValue + nl +
                                  "Date:               " + DateOfAddition + nl;

                            switch (sampleType)
                            {
                                case "ATAC-Seq":
                                    string TransposaseUnit = reader["Transposase_Unit"].ToString();
                                    string PCRCycles = reader["PCR_Cycles"].ToString();
                                    ret += "Transposase Unit:   " + TransposaseUnit + nl +
                                           "PCR Cycles:         " + PCRCycles;
                                    break;
                                case "Hi-C":
                                    string RestrictionEnzyme = reader["Restriction_Enzyme"].ToString();
                                    PCRCycles = reader["PCR_Cycles"].ToString();
                                    ret += "Restriction Enzyme: " + RestrictionEnzyme + nl +
                                           "PCR Cycles:         " + PCRCycles + nl;
                                    break;
                                case "RNA-Seq":
                                    string PrepType = reader["Prep_Type"].ToString();
                                    string RIN = reader["RIN"].ToString();
                                    ret += "Prep Type:          " + PrepType + nl +
                                           "RIN:                " + RIN + nl;
                                    break;
                                case "ChIP-Seq":
                                    string Antibody = reader["Antibody"].ToString();
                                    string AntibodyLot = reader["Antibody_Lot"].ToString();
                                    string AntibodyCatalogueNumber = reader["Antibody_Catalogue_Number"].ToString();
                                    ret += "Atibody:            " + Antibody + nl +
                                           "Antibody Lot:       " + AntibodyLot + nl +
                                           "Antibody Cat. Nr:   " + AntibodyCatalogueNumber + nl;
                                    break;
                            }

                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return ret;
        }

        /*public void GetSampleByStringValue(string value, string spParameter, string storedProcedure)
        {                                                                 
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand command = new SqlCommand(storedProcedure, con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter(spParameter, value));

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())//runs through this loop for any sample id relevant to the value looked for
                        {
                            int sampleID = Int32.Parse(reader["Sample_ID"].ToString());
                            string sampleType = reader["Sample_Type"].ToString();
                            GetSampleWithStoredProcedure(sampleID, sampleType);
                        }
                    }
                }
                catch (SqlException e)
                {
                    //Console.WriteLine(e.Message);
                }
            }
        }
        */



        /* private void GetSampleWithStoredProcedure(int SampleID, string SampleType)
         {
             using (SqlConnection con = new SqlConnection(connectionString))
             {
                 try
                 {
                     SqlCommand cmd1 = new SqlCommand("spGetSampleInfoFromIDAndType", con);
                     cmd1.CommandType = CommandType.StoredProcedure;

                    cmd1.Parameters.Add("@Sample_ID", SqlDbType.Int).Value = SampleID;
                    cmd1.Parameters.Add("@Sample_Type", SqlDbType.VarChar).Value = SampleType;

                     con.Open();
                     SqlDataReader reader = cmd1.ExecuteReader();

                     if (reader.HasRows)
                     {
                         while (reader.Read())
                         {
                             string xampleType = reader["Sample_Type"].ToString();//probably not needed as the sampletype is imported as an arg through the method parameters
                             string GenomeType = reader["Genome_Type"].ToString();
                             string CellType = reader["Cell_Type"].ToString();
                             string Treatment = reader["Treatment"].ToString();
                             string Condition = reader["Condition"].ToString();
                             string Comments = reader["Comments"].ToString();
                             string Concentration = reader["Concentration"].ToString();
                             string Volume = reader["Volume"].ToString();
                             string Initials = reader["Initials"].ToString();
                             string PiValue = reader["Pi_Value"].ToString();
                             string DateOfAddition = reader["Date_Of_Addition"].ToString();

                             Console.WriteLine("SAMPLE ID:          " + SampleID);
                             Console.WriteLine("Sample Type:        " + SampleType);
                             Console.WriteLine("Genome Type:        " + GenomeType);
                             Console.WriteLine("Cell Type:          " + CellType);
                             Console.WriteLine("Treatment:          " + Treatment);
                             Console.WriteLine("Condition:          " + Condition);
                             Console.WriteLine("Comments:           " + Comments);
                             Console.WriteLine("Concentration:      " + Concentration);
                             Console.WriteLine("Volume:             " + Volume);
                             Console.WriteLine("Initials:           " + Initials);
                             Console.WriteLine("PI:                 " + PiValue);
                             Console.WriteLine("Date:               " + DateOfAddition);

                             switch (SampleType)
                             {
                                 case "ATAC-Seq":
                                     string TransposaseUnit = reader["Transposase_Unit"].ToString();
                                     string PCRCycles = reader["PCR_Cycles"].ToString();
                                     Console.WriteLine("Transposase Unit:   " + TransposaseUnit);
                                     Console.WriteLine("PCR Cycles:         " + PCRCycles);
                                     break;
                                 case "HI-C":
                                     string RestrictionEnzyme = reader["Restriction_Enzyme"].ToString();
                                     PCRCycles = reader["PCR_Cycles"].ToString();
                                     Console.WriteLine("Restriction Enzyme: " + RestrictionEnzyme);
                                     Console.WriteLine("PCR Cycle:          " + PCRCycles);
                                     break;
                                 case "RNA-Seq":
                                     string PrepType = reader["Prep_Type"].ToString();
                                     string RIN = reader["RIN"].ToString();
                                     Console.WriteLine("Prep Type:          " + PrepType);
                                     Console.WriteLine("RIN:                " + RIN);
                                     break;
                                 case "ChIP-Seq":
                                     string Antibody = reader["Antibody"].ToString();
                                     string AntibodyLot = reader["Antibody_Lot"].ToString();
                                     string AntibodyCatalogueNumber = reader["Antibody_Catalogue_Number"].ToString();
                                     Console.WriteLine("Atibody:            " + Antibody);
                                     Console.WriteLine("Antibody Lot:       " + AntibodyLot);
                                     Console.WriteLine("Antibody Cat. Nr:   " + AntibodyCatalogueNumber);
                                     break;
                             }
                             Console.WriteLine("\n"); 
                         }
                     }
                 }
                 catch (SqlException e)
                 {
                     Console.WriteLine(e.Message);
                 }
             }
         }*/
    }
}


