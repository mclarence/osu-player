using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using BMAPI.v1;
using BMAPI.v1.Events;
using System.Threading.Tasks;

namespace osu_player
{
    class DatabaseHandler
    {


        SQLiteConnection db;
        public int numberofBeatmaps;
        public int currentProcessedBeatmap;
        public DatabaseHandler()
        {
            string dbPath = Application.StartupPath + @"\music.sqlite";
            if (File.Exists(dbPath)) {
                db = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            } else
            {
                SQLiteConnection.CreateFile(dbPath);
                db = new SQLiteConnection($"Data Source={dbPath};Version=3;");
                db.Open();
                string sql = "create table songs (id INTEGER PRIMARY KEY AUTOINCREMENT, title varchar(255), artist varchar(255), mp3 varchar(255));";
                SQLiteCommand createTable = new SQLiteCommand(sql, db);
                createTable.ExecuteNonQuery();
                db.Close();
            

            }

        }

        public void addSongs(string path)
        {
            db.Open();
            if (db == null | !Directory.Exists(path))
            {
                throw new Exception();
            } else
            {
                string[] beatmapFolders = Directory.GetDirectories(path);
                numberofBeatmaps = beatmapFolders.Length;
                currentProcessedBeatmap = 0;
                List<Song> Songs = new List<Song>();
                foreach (string beatmapFolder in beatmapFolders)
                {
                    if (beatmapFolder.EndsWith(".sync"))
                    {
                        continue;
                    }
                    bool grabbedMetadata = false;
                    bool grabbedMP3 = false;
                    Song song = new Song();
                    foreach (string file in Directory.GetFiles(beatmapFolder))
                    {

                        if (file.ToLower().EndsWith(".mp3") & grabbedMP3 == false)
                        {
                            song.mp3 = file;
                            grabbedMP3 = true;
                        }
                        else if (file.ToLower().EndsWith(".osu") & grabbedMetadata == false)
                        {

                            Beatmap beatmap = new Beatmap(file);

                            song.title = beatmap.Title.Replace("\"", string.Empty);
                            song.artist = beatmap.Artist.Replace("\"", string.Empty);
                            grabbedMetadata = true;
                        }
                        else
                        {
                            continue;
                        }
                      

                        

                    }
                    
                    Songs.Add(song);
                    currentProcessedBeatmap++;

                }

                StringBuilder sql = new StringBuilder("insert into songs (title, artist, mp3) values");

                int count = 0;
                foreach (Song song in Songs)
                {
                    if (count > 0) {
                        sql.Append($", (\"{song.title}\", \"{song.artist}\", \"{song.mp3}\")");
                    } else {
                        sql.Append($" (\"{song.title}\", \"{song.artist}\", \"{song.mp3}\")");
                    }
                    count++;
                }
                SQLiteCommand insertSongs = new SQLiteCommand(sql.ToString(), db);
                insertSongs.ExecuteNonQuery();
                
            }
            db.Close();
        }

        public List<Song> readSongs()
        {
        
            db.Open();
            string sql = "select * from songs order by id desc";
            SQLiteCommand readDB = new SQLiteCommand(sql, db);
            SQLiteDataReader reader = readDB.ExecuteReader();
            List<Song> songs = new List<Song>();
            while (reader.Read())
            {
                Song song = new Song();
                song.id = Convert.ToInt32(reader["id"].ToString());
                song.title = reader["title"].ToString();
                song.artist = reader["artist"].ToString();
                song.mp3 = reader["mp3"].ToString();
                songs.Add(song);
            }
            db.Close();
            return songs;
        }

    }
    
}
