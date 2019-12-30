using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using WMPLib;
namespace osu_player
{
    public partial class Form1 : Form
    {
        List<Song> Songs;
        DatabaseHandler database;
        Song currentSong;
        WindowsMediaPlayerClass player;
        public Form1()
        {
            player = new WindowsMediaPlayerClass();
            player.OpenStateChange += Player_OpenStateChange; 
            database = new DatabaseHandler();
            Songs = database.readSongs();
            InitializeComponent();
        }

        private void Player_OpenStateChange(int NewState)
        {
            if (NewState == 13)
            {
                tb_mediaPosition.Enabled = true;
                lbl_duration.Text = player.currentMedia.durationString;
                tb_mediaPosition.Maximum = (int)player.currentMedia.duration;
                timer2.Enabled = true;
            }



        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Song> Songs = database.readSongs();
            objectListView1.SetObjects(Songs);

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            objectListView1.SetObjects(Songs);
            trackBar1.Value = player.volume;
        }

        private void objectListView1_ItemActivate(object sender, EventArgs e)
        {
            if (currentSong == null) {
                Song song = Songs.Find(x => x.id == Convert.ToInt32(((Song)objectListView1.SelectedObject).id));
                play(song);
            }
            else
            {
                currentSong.playingIcon = null;
                objectListView1.UpdateObject(currentSong);
                Song song = Songs.Find(x => x.id == Convert.ToInt32(((Song)objectListView1.SelectedObject).id));
                play(song);
            }

        }

        private void play(Song song)
        {
            player.URL = song.mp3;
            song.playingIcon = "-->";
            currentSong = song;
            btn_pause.Enabled = true;
            btn_stop.Enabled = true;
            objectListView1.UpdateObject(song);
            lbl_status.Text = $"Playing: {song.title} by {song.artist} ";
            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void importSongsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select the osu! Songs directory.";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                lbl_progressbar.Visible = true;
                lbl_progress.Visible = true;
                timer1.Enabled = true;
                bgw_addSongs.RunWorkerAsync(dlg.SelectedPath);


            }



        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void bgw_addSongs_DoWork(object sender, DoWorkEventArgs e)
        {

            database.addSongs((string)e.Argument);
            Songs = database.readSongs();

        }


        private void bgw_addSongs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            objectListView1.SetObjects(Songs);
            lbl_progressbar.Visible = false;
            lbl_progress.Visible = false;
            timer1.Enabled = false;
            MessageBox.Show("Import completed!");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_status.Text = "Status: Importing...";

            lbl_progressbar.Maximum = database.numberofBeatmaps;
            lbl_progressbar.Value = database.currentProcessedBeatmap;


            lbl_progress.Text = Convert.ToInt32(database.currentProcessedBeatmap) + "/" + Convert.ToInt32(database.numberofBeatmaps);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            player.controls.play();
            lbl_status.Text = $"Playing: {currentSong.title} by {currentSong.artist} ";
            btn_play.Enabled = false;
            btn_pause.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            player.controls.pause();
            lbl_status.Text = $"Paused: {currentSong.title} by {currentSong.artist} ";
            btn_pause.Enabled = false;
            btn_play.Enabled = true;

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            player.controls.stop();
            timer2.Enabled = false;
            tb_mediaPosition.Enabled = false;
            tb_mediaPosition.Maximum = 0;
            tb_mediaPosition.Value = 0;
            lbl_duration.Text = "00:00";
            lbl_elapsed.Text = "00:00";
            lbl_status.Text = $"Nothing is playing";
            btn_play.Enabled = false;
            btn_pause.Enabled = false;
            btn_stop.Enabled = false;
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            lbl_elapsed.Text = player.currentPositionString;
            tb_mediaPosition.Value = (int)player.currentPosition;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            player.currentPosition = tb_mediaPosition.Value; 
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            bool found = false;
            foreach (Song song in objectListView1.Objects)
            {
                if (song.playingIcon != null) {
                    found = true;
                    continue;
                }
                if (found == true)
                {
                    song.playingIcon = "-->";
                    player.URL = song.mp3;
                    objectListView1.UpdateObject(song);
                    break;
                }
            }
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            player.volume = trackBar1.Value;
        }
    }
}