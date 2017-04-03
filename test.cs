   static class SubstringContain {
        public static bool SubstringContains(this string fullstring, string substing) {
            if (fullstring.IndexOf(substing, 0, fullstring.Length) != -1) {
                return true;
            }
            return false;
        }
    }         

DataSet sql_creator = new DataSet();
            System.IO.FileStream xml_reader = new System.IO.FileStream(@"C:\Users\800PatSa\Desktop\AntiMoneyLaundryXML\Dow_Jones_PEP_V4.xsd", System.IO.FileMode.Open);
            try {
                sql_creator.ReadXml(xml_reader);
            } catch (Exception ex) {
                Console.WriteLine("XML reading error: " + ex.Message);
            } finally {
                xml_reader.Close();
            }

            List<string> queries = new List<string>();

            foreach (DataTable table in sql_creator.Tables) {
                string part_query;
                foreach (DataColumn column in table.Columns) {
                    //primary key
                    //Table PFA -> column name PFA_id
                    if (column.ColumnName.ToLower().SubstringContains(table.TableName.ToLower()+'_')) {

                    }
                    //foreign key
                    //Table PFA -> column name SomeThing_id
                    else if (column.ColumnName.ToLower().SubstringContains("id")) {

                    }
                    //

                }
                foreach (DataRelation column in table.ChildRelations) {
                    //foreign key
                    if (column.RelationName.ToLower().SubstringContains(table.TableName.ToLower()+'_')) {

                    }
                }



            }
        }
