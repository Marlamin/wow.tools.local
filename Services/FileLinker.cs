using Microsoft.Data.Sqlite;
using WoWFormatLib.FileReaders;

namespace wow.tools.local.Services
{
    public static class Linker
    {
        public struct LinkedFile
        {
            public uint fileDataID;
            public string linkType;
        }


        private static SqliteCommand insertCmd;
        private static HashSet<int> existingParents = new HashSet<int>();
        static Linker()
        {
            insertCmd = new SqliteCommand("INSERT INTO wow_rootfiles_links VALUES (@parent, @child, @type)", SQLiteDB.dbConn);
            insertCmd.Parameters.AddWithValue("@parent", 0);
            insertCmd.Parameters.AddWithValue("@child", 0);
            insertCmd.Parameters.AddWithValue("@type", "");
            insertCmd.Prepare();

            using (var cmd = SQLiteDB.dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT parent FROM wow_rootfiles_links";

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    existingParents.Add(int.Parse(reader["parent"].ToString()));
                }

                reader.Close();
            }

            WoWFormatLib.Utils.CASC.InitCasc(CASC.cascHandler);
        }

        private static void insertEntry(SqliteCommand cmd, uint fileDataID, string desc)
        {
            if (fileDataID == 0)
                return;

            try
            {
                cmd.Parameters[1].Value = fileDataID;
                cmd.Parameters[2].Value = desc;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("UNIQUE constraint failed"))
                {
                    Console.WriteLine("Error inserting FDID (" + desc + "): " + e.Message);
                }
            }
        }

        public static void LinkM2(uint fileDataID)
        {
            if (!CASC.FileExists(fileDataID))
                return;

            if (existingParents.Contains((int)fileDataID))
                return;

            try
            {
                var reader = new M2Reader();
                reader.LoadM2(fileDataID, false);

                existingParents.Add((int)fileDataID);

                insertCmd.Parameters[0].Value = fileDataID;

                if (reader.model.textureFileDataIDs != null)
                {
                    foreach (var textureID in reader.model.textureFileDataIDs)
                    {
                        insertEntry(insertCmd, textureID, "m2 texture");
                    }
                }

                if (reader.model.animFileDataIDs != null)
                {
                    foreach (var animFileID in reader.model.animFileDataIDs)
                    {
                        insertEntry(insertCmd, animFileID.fileDataID, "m2 anim");
                    }
                }

                if (reader.model.skinFileDataIDs != null)
                {
                    foreach (var skinFileID in reader.model.skinFileDataIDs)
                    {
                        insertEntry(insertCmd, skinFileID, "m2 skin");
                    }
                }

                if (reader.model.boneFileDataIDs != null)
                {
                    foreach (var boneFileID in reader.model.boneFileDataIDs)
                    {
                        insertEntry(insertCmd, boneFileID, "m2 bone");
                    }
                }

                if (reader.model.recursiveParticleModelFileIDs != null)
                {
                    foreach (var rpID in reader.model.recursiveParticleModelFileIDs)
                    {
                        insertEntry(insertCmd, rpID, "m2 recursive particle");
                    }
                }

                if (reader.model.geometryParticleModelFileIDs != null)
                {
                    foreach (var gpID in reader.model.geometryParticleModelFileIDs)
                    {
                        insertEntry(insertCmd, gpID, "m2 geometry particle");
                    }
                }

                insertEntry(insertCmd, reader.model.skelFileID, "m2 skel");
                insertEntry(insertCmd, reader.model.physFileID, "m2 phys");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void LinkWMO(uint fileDataID)
        {
            if (!CASC.FileExists(fileDataID))
                return;

            if (existingParents.Contains((int)fileDataID))
                return;

            try
            {
                var reader = new WMOReader();
                var wmo = new WoWFormatLib.Structs.WMO.WMO();
                try
                {
                    wmo = reader.LoadWMO(fileDataID);
                }
                catch (NotSupportedException e)
                {
                    Console.WriteLine("[WMO] " + fileDataID + " is a group WMO, skipping..");
                    CASC.SetFileType((int)fileDataID, "gwmo");
                    return;
                }

                existingParents.Add((int)fileDataID);

                insertCmd.Parameters[0].Value = fileDataID;

                var inserted = new List<uint>();

                if (wmo.groupFileDataIDs != null)
                {
                    foreach (var groupFileDataID in wmo.groupFileDataIDs)
                    {
                        insertEntry(insertCmd, groupFileDataID, "wmo group");
                        CASC.SetFileType((int)groupFileDataID, "gwmo");
                    }
                }

                if (wmo.doodadIds != null)
                {
                    foreach (var doodadID in wmo.doodadIds)
                    {
                        if (inserted.Contains(doodadID))
                            continue;

                        inserted.Add(doodadID);

                        insertEntry(insertCmd, doodadID, "wmo doodad");
                    }
                }

                if (wmo.textures == null && wmo.materials != null)
                {
                    foreach (var material in wmo.materials)
                    {
                        if (material.texture1 != 0 && !inserted.Contains(material.texture1))
                        {
                            inserted.Add(material.texture1);
                            insertEntry(insertCmd, material.texture1, "wmo texture");
                        }

                        if (material.texture2 != 0 && !inserted.Contains(material.texture2))
                        {
                            inserted.Add(material.texture2);
                            insertEntry(insertCmd, material.texture2, "wmo texture");
                        }

                        if (material.texture3 != 0 && !inserted.Contains(material.texture3))
                        {
                            inserted.Add(material.texture3);
                            insertEntry(insertCmd, material.texture3, "wmo texture");
                        }

                        if (material.shader == 23)
                        {
                            if (material.color3 != 0 && !inserted.Contains(material.color3))
                            {
                                inserted.Add(material.color3);
                                insertEntry(insertCmd, material.color3, "wmo texture");
                            }

                            if (material.flags3 != 0 && !inserted.Contains(material.flags3))
                            {
                                inserted.Add(material.flags3);
                                insertEntry(insertCmd, material.flags3, "wmo texture");
                            }

                            if (material.runtimeData0 != 0 && !inserted.Contains(material.runtimeData0))
                            {
                                inserted.Add(material.runtimeData0);
                                insertEntry(insertCmd, material.runtimeData0, "wmo texture");
                            }

                            if (material.runtimeData1 != 0 && !inserted.Contains(material.runtimeData1))
                            {
                                inserted.Add(material.runtimeData1);
                                insertEntry(insertCmd, material.runtimeData1, "wmo texture");
                            }

                            if (material.runtimeData2 != 0 && !inserted.Contains(material.runtimeData2))
                            {
                                inserted.Add(material.runtimeData2);
                                insertEntry(insertCmd, material.runtimeData2, "wmo texture");
                            }

                            if (material.runtimeData3 != 0 && !inserted.Contains(material.runtimeData3))
                            {
                                inserted.Add(material.runtimeData3);
                                insertEntry(insertCmd, material.runtimeData3, "wmo texture");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }

        public static void LinkADT(uint fileDataID)
        {
            throw new NotImplementedException();
        }

        public static void LinkWDT(uint fileDataID)
        {
            throw new NotImplementedException();
        }

        public static void Link(bool fullrun = false)
        {
            #region M2
            if (!CASC.TypeMap.TryGetValue("m2", out var m2ids))
            {
                Console.WriteLine("Unable to get M2 files, make sure types have been detected at least once.");
                return;
            }

            var processedM2s = 0;

            var transaction = SQLiteDB.dbConn.BeginTransaction();
            insertCmd.Transaction = transaction;

            foreach (var m2 in m2ids)
            {
                LinkM2((uint)m2);

                processedM2s++;

                if(processedM2s % 1000 == 0)
                {
                    transaction.Commit();
                    transaction = SQLiteDB.dbConn.BeginTransaction();
                    insertCmd.Transaction = transaction;
                }

                Console.Write("\rM2s processed: " + processedM2s + "/" + m2ids.Count);
            }

            transaction.Commit();

            Console.WriteLine();
            #endregion

            #region WMO
            if (!CASC.TypeMap.TryGetValue("wmo", out var wmoids))
            {
                Console.WriteLine("Unable to get WMO files, make sure types have been detected at least once.");
                return;
            }

            var processedWMOs = 0;
            transaction = SQLiteDB.dbConn.BeginTransaction();
            insertCmd.Transaction = transaction;
            foreach (var wmo in wmoids)
            {
                LinkWMO((uint)wmo);

                processedWMOs++;

                if (processedM2s % 1000 == 0)
                {
                    transaction.Commit();
                    transaction = SQLiteDB.dbConn.BeginTransaction();
                    insertCmd.Transaction = transaction;
                }

                Console.Write("\rWMOs processed: " + processedWMOs + "/" + wmoids.Count);
            }

            transaction.Commit();

            Console.WriteLine();
            #endregion

            #region Maps
            if (!CASC.TypeMap.TryGetValue("wdt", out var wdtids))
            {
                Console.WriteLine("Unable to get WDT files, make sure types have been detected at least once.");
                return;
            }

            using (var cmd = SQLiteDB.dbConn.CreateCommand())
            {
                foreach (var wdtid in wdtids)
                {

                    if (!CASC.Listfile.TryGetValue(wdtid, out var wdtFilename))
                    {
                        Console.WriteLine("Unable to find filename for WDT " + wdtid + ", skipping linking!");
                        continue;
                    }

                    if (wdtFilename.Contains("_mpv") || wdtFilename.Contains("_lgt") || wdtFilename.Contains("_occ") || wdtFilename.Contains("_fogs"))
                        continue;

                    Console.WriteLine("[WDT] Loading " + wdtid + " (" + wdtFilename + ")");

                    insertCmd.Parameters[0].Value = wdtid;

                    try
                    {
                        var wdtreader = new WDTReader();
                        wdtreader.LoadWDT((uint)wdtid);

                        transaction = SQLiteDB.dbConn.BeginTransaction();
                        insertCmd.Transaction = transaction;

                        if (wdtreader.wdtfile.modf.id != 0 && !existingParents.Contains(wdtid))
                        {
                            insertEntry(insertCmd, wdtreader.wdtfile.modf.id, "wdt wmo");
                        }

                        foreach (var records in wdtreader.wdtfile.tileFiles)
                        {
                            // Switch to WDT fdid (previous could be ADT fdid)
                            insertCmd.Parameters[0].Value = wdtid;

                            if (!existingParents.Contains(wdtid))
                            {
                                insertEntry(insertCmd, records.Value.rootADT, "root adt");
                                insertEntry(insertCmd, records.Value.tex0ADT, "tex0 adt");
                                insertEntry(insertCmd, records.Value.lodADT, "lod adt");
                                insertEntry(insertCmd, records.Value.obj0ADT, "obj0 adt");
                                insertEntry(insertCmd, records.Value.obj1ADT, "obj1 adt");
                                insertEntry(insertCmd, records.Value.mapTexture, "map texture");
                                insertEntry(insertCmd, records.Value.mapTextureN, "mapn texture");
                                insertEntry(insertCmd, records.Value.minimapTexture, "minimap texture");
                            }

                            if (records.Value.rootADT == 0)
                                continue;

                            if (existingParents.Contains((int)records.Value.rootADT))
                                continue;

                            // Switch to ADT FDID                            
                            insertCmd.Parameters[0].Value = records.Value.rootADT;

                            var inserted = new List<uint>();

                            var adtreader = new ADTReader();
                            try
                            {
                                adtreader.LoadADT(wdtreader.wdtfile, records.Key.Item1, records.Key.Item2);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }

                            if (adtreader.adtfile.objects.m2Names.filenames != null)
                            {
                                Console.WriteLine(records.Value.rootADT + " is still using old filenames, skipping!");
                            }
                            else
                            {
                                foreach (var worldmodel in adtreader.adtfile.objects.worldModels.entries)
                                {
                                    if (inserted.Contains(worldmodel.mwidEntry))
                                        continue;

                                    inserted.Add(worldmodel.mwidEntry);
                                    insertEntry(insertCmd, worldmodel.mwidEntry, "adt worldmodel");
                                }

                                foreach (var doodad in adtreader.adtfile.objects.models.entries)
                                {
                                    if (inserted.Contains(doodad.mmidEntry))
                                        continue;

                                    insertEntry(insertCmd, doodad.mmidEntry, "adt doodad");
                                    inserted.Add(doodad.mmidEntry);
                                }
                            }

                            foreach (var texture in adtreader.adtfile.diffuseTextureFileDataIDs)
                            {
                                if (texture == 0)
                                    continue;

                                insertEntry(insertCmd, texture, "adt diffuse texture");
                            }

                            foreach (var texture in adtreader.adtfile.heightTextureFileDataIDs)
                            {
                                if (texture == 0)
                                    continue;

                                insertEntry(insertCmd, texture, "adt height texture");
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    }
                }
            }
            #endregion
        }
    }
}
