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
                if (!e.Message.StartsWith("Duplicate entry"))
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
            for (var i = 0; i < m2ids.Count; i++)
            {
                LinkM2((uint)m2ids[i]);

                processedM2s++;

                Console.Write("\rM2s processed: " + processedM2s + "/" + m2ids.Count);
            }

            Console.WriteLine();
            #endregion

            #region WMO
            if (!CASC.TypeMap.TryGetValue("wmo", out var wmoids))
            {
                Console.WriteLine("Unable to get WMO files, make sure types have been detected at least once.");
                return;
            }

            var processedWMOs = 0;
            for (var i = 0; i < wmoids.Count; i++)
            {
                LinkWMO((uint)wmoids[i]);

                processedWMOs++;

                Console.Write("\rWMOs processed: " + processedWMOs + "/" + wmoids.Count);
            }

            Console.WriteLine();
            #endregion

            return;
            #region WDT
            var wdtids = new List<uint>();
            var wdtfullnamemap = new Dictionary<string, uint>();
            using (var cmd = SQLiteDB.dbConn.CreateCommand())
            {
                Console.WriteLine("[WDT] Generating list of WDT files..");
                cmd.CommandText = "SELECT id, filename from wow_rootfiles WHERE type = 'wdt' AND filename IS NOT NULL ORDER BY id DESC";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var filename = (string)reader["filename"];
                    var wdtid = uint.Parse(reader["id"].ToString());
                    if (filename.Contains("_mpv") || filename.Contains("_lgt") || filename.Contains("_occ") || filename.Contains("_fogs"))
                        continue;
                    wdtfullnamemap.Add(filename, wdtid);
                }
            }

            using (var cmd = SQLiteDB.dbConn.CreateCommand())
            {
                if (fullrun)
                {
                    cmd.CommandText = "SELECT id, filename from wow_rootfiles WHERE type = 'wdt' ORDER BY id DESC";
                }
                else
                {
                    Console.WriteLine("[WDT] Generating list of files to process..");
                    cmd.CommandText = "SELECT id, filename from wow_rootfiles WHERE type = 'wdt' AND id NOT IN (SELECT parent FROM wow_rootfiles_links) ORDER BY id DESC";
                }
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //var filename = (string)reader["filename"];
                    var wdtid = uint.Parse(reader["id"].ToString());
                    //if (filename.Contains("_mpv") || filename.Contains("_lgt") || filename.Contains("_occ") || filename.Contains("_fogs"))
                    //   continue;
                    wdtids.Add(wdtid);
                }

                reader.Close();

                foreach (var wdtid in wdtids)
                {
                    Console.WriteLine("[WDT] Loading " + wdtid);

                    insertCmd.Parameters[0].Value = wdtid;
                    try
                    {
                        var wdtreader = new WDTReader();
                        wdtreader.LoadWDT(wdtid);

                        if (wdtreader.wdtfile.modf.id != 0)
                        {
                            Console.WriteLine("WDT has WMO ID: " + wdtreader.wdtfile.modf.id);
                            insertEntry(insertCmd, wdtreader.wdtfile.modf.id, "wdt wmo");
                        }

                        foreach (var records in wdtreader.stringTileFiles)
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
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    }

                }
            }
            #endregion

            #region ADT
            var adtids = new Dictionary<uint, Dictionary<(byte, byte), uint>>();
            var wdtmapping = new Dictionary<string, uint>();

            using (var cmd = SQLiteDB.dbConn.CreateCommand())
            {
                if (fullrun)
                {
                    cmd.CommandText = " SELECT id, filename from wow_rootfiles WHERE filename LIKE '%adt' AND filename NOT LIKE '%_obj0.adt' AND filename NOT LIKE '%_obj1.adt' AND filename NOT LIKE '%_lod.adt' AND filename NOT LIKE '%tex0.adt' AND filename NOT LIKE '%tex1.adt' ORDER BY id DESC ";
                }
                else
                {
                    Console.WriteLine("[ADT] Generating list of files to process..");
                    cmd.CommandText = " SELECT id, filename from wow_rootfiles WHERE filename LIKE '%adt' AND filename NOT LIKE '%_obj0.adt' AND filename NOT LIKE '%_obj1.adt' AND filename NOT LIKE '%_lod.adt' AND filename NOT LIKE '%tex0.adt' AND filename NOT LIKE '%tex1.adt' AND id NOT IN (SELECT parent FROM wow_rootfiles_links) ORDER BY id DESC";
                }
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var filename = (string)reader["filename"];
                    var mapname = filename.Replace("world/maps/", "").Substring(0, filename.Replace("world/maps/", "").IndexOf("/"));
                    var exploded = Path.GetFileNameWithoutExtension(filename).Split('_');

                    for (var i = 0; i < exploded.Length; i++)
                    {
                        //Console.WriteLine(i + ": " + exploded[i]);
                    }

                    byte tileX = 0;
                    byte tileY = 0;

                    if (!byte.TryParse(exploded[exploded.Length - 2], out tileX) || !byte.TryParse(exploded[exploded.Length - 1], out tileY))
                    {
                        Console.WriteLine("An error occured converting coordinates from " + filename + " to bytes");
                        continue;
                    }

                    if (!wdtmapping.ContainsKey(mapname))
                    {
                        var wdtname = "world/maps/" + mapname + "/" + mapname + ".wdt";
                        if (!wdtfullnamemap.ContainsKey(wdtname))
                        {
                            Console.WriteLine("Unable to get filedataid for " + mapname + ", skipping...");
                            wdtmapping.Remove(mapname);
                            continue;
                        }
                        wdtmapping.Add(mapname, wdtfullnamemap[wdtname]);
                        if (wdtmapping[mapname] == 0)
                        {
                            // TODO: Support WDTs removed in current build
                            Console.WriteLine("Unable to get filedataid for " + mapname + ", skipping...");
                            wdtmapping.Remove(mapname);
                            continue;
                            /*
                            var wdtconn = new MySqlConnection(File.ReadAllText("connectionstring.txt"));
                            wdtconn.Open();
                            using (var wdtcmd = wdtconn.CreateCommand())
                            {
                                wdtcmd.CommandText = "SELECT id from wow_rootfiles WHERE filename = '" + wdtname + "'";
                                var wdtread = wdtcmd.ExecuteReader();
                                while (wdtread.Read())
                                {
                                    wdtmapping[mapname] = uint.Parse(wdtread["id"].ToString());
                                }
                            }
                            wdtconn.Close();*/
                        }

                        adtids.Add(wdtmapping[mapname], new Dictionary<(byte, byte), uint>());
                    }

                    var id = uint.Parse(reader["id"].ToString());

                    if (id == 0)
                    {
                        Console.WriteLine("Root ADT " + tileX + ", " + tileY + " with ID 0 on WDT " + wdtmapping[mapname]);
                        continue;
                    }

                    if (wdtmapping.ContainsKey(mapname))
                    {
                        adtids[wdtmapping[mapname]].Add((tileX, tileY), id);
                    }
                }

                reader.Close();

                foreach (var wdtid in adtids)
                {
                    foreach (var adtid in wdtid.Value)
                    {
                        var inserted = new List<uint>();
                        Console.WriteLine("[ADT] Loading " + adtid.Key.Item1 + ", " + adtid.Key.Item2 + "(" + adtid.Value + ")");

                        insertCmd.Parameters[0].Value = adtid.Value;

                        var adtreader = new ADTReader();
                        try
                        {
                            adtreader.LoadADT(wdtid.Key, adtid.Key.Item1, adtid.Key.Item2);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            continue;
                        }

                        if (adtreader.adtfile.objects.m2Names.filenames != null)
                        {
                            Console.WriteLine(adtid + " is still using old filenames, skipping!");
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
                    }
                }
            }
            #endregion
        }
    }
}
