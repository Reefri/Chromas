using Godot;
using System.Collections.Generic;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation {
	
	public partial class SoundManager : Node
	{

		static private SoundManager instance;
		static private PackedScene factory = GD.Load<PackedScene>("res://Scenes/Manager/SoundManager.tscn");


		[ExportGroup("Musics")]

        [Export] public AudioStreamPlayer ambiance;
		[Export] public AudioStreamPlayer level;
		[Export] public AudioStreamPlayer boss;
		[Export] public AudioStreamPlayer UI;






        [ExportCategory("Sounds")]




        [ExportGroup("Boss")]


        [Export] public AudioStreamPlayer bossExplosion;
        [Export] public AudioStreamPlayer bossPreExplosion;
        [Export] public AudioStreamPlayer bossShoot;




        [ExportGroup("Collectables")]

        [Export] public AudioStreamPlayer smartBombCollectable;
        [Export] public AudioStreamPlayer healCollectable;
        [Export] public AudioStreamPlayer levelUpCollectable;



        [ExportGroup("Enemy")]

        [Export] public AudioStreamPlayer enemyOneExplosionOne;
        [Export] public AudioStreamPlayer enemyOneExplosionTwo;
        [Export] public AudioStreamPlayer enemyOneExplosionThree;
        [Export] public AudioStreamPlayer enemyOneExplosionFour;
        [Export] public AudioStreamPlayer enemyExplosion;
        [Export] public AudioStreamPlayer enemyShoot;



        [ExportGroup("Other")]

        [Export] public AudioStreamPlayer bomb;
        [Export] public AudioStreamPlayer obstacleDestruction;
        [Export] public AudioStreamPlayer construction;



        [ExportGroup("Player")]

        [Export] public AudioStreamPlayer loselife;
        [Export] public AudioStreamPlayer playerExplosion;
        [Export] public AudioStreamPlayer playerShoot0;
        [Export] public AudioStreamPlayer playerShoot1;
        [Export] public AudioStreamPlayer playerShoot2;
        [Export] public AudioStreamPlayer playerShoot3;
        [Export] public AudioStreamPlayer colorSwap;
        [Export] public AudioStreamPlayer camouflage_IN;
        [Export] public AudioStreamPlayer camouflage_OUT;



        [ExportGroup("UI")]

        [Export] public AudioStreamPlayer gameOverJingle;
        [Export] public AudioStreamPlayer uiClick;
        [Export] public AudioStreamPlayer uiPause;
        [Export] public AudioStreamPlayer uiStart;
        [Export] public AudioStreamPlayer winJingle;


        private List<AudioStreamPlayer> enemyOneExplosionList;


        private List<AudioStreamPlayer> playerShootList;


        Tween levelDBTween;
        Tween uiDBTween;

        float musicDB = 0;
        float soundDB = 0;

        private SoundManager():base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(SoundManager) + " Instance already exist, destroying the last added.");
				return;
			}

			instance = this;	
		}

		static public SoundManager GetInstance()
		{
			if (instance == null) instance = (SoundManager)factory.Instantiate();
			return instance;

		}

        public override void _Ready()
        {

            enemyOneExplosionList = new List<AudioStreamPlayer>
            { 
                enemyOneExplosionOne, 
                enemyOneExplosionTwo, 
                enemyOneExplosionThree, 
                enemyOneExplosionFour,
            };

            playerShootList = new List<AudioStreamPlayer>
            {

                playerShoot0,
                playerShoot1,
                playerShoot2,
                playerShoot3,
            };

            musicDB -= 20;
            soundDB -= 20;

            foreach(Node lNode in GetNode("Sounds").GetChildren())
            {
                foreach (AudioStreamPlayer lSoud in lNode.GetChildren())
                    lSoud.VolumeDb = soundDB;
            }
            
            foreach(AudioStreamPlayer lSounds in GetNode("Musics").GetChildren())
            {
                lSounds.VolumeDb = musicDB;
            }


        }


        //##########################################################################
        //#########################/          MUSIC         /#######################
        //##########################################################################

        public void PlayAmbiance()
        {
            ambiance.Play();
        }

        public void PlayLevel()
        {
            level.Play();
        }

        public void StopLevel()
        {
            levelDBTween?.Kill();
            levelDBTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out);

            levelDBTween.TweenProperty(level, "volume_db", -80, 1);

            levelDBTween.SetPauseMode(Tween.TweenPauseMode.Process);

        }
        public void ResumeLevel()
        {
            levelDBTween?.Kill();
            levelDBTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out);

            levelDBTween.TweenProperty(level, "volume_db", musicDB, 1);

            levelDBTween.SetPauseMode(Tween.TweenPauseMode.Process);

        }

        public void PlayBoss()
        {
            boss.Play();
        }

        public void PlayUI()
        {
            UI.Play();
        }

        public void StopUI()
        {
            uiDBTween?.Kill();
            uiDBTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out);

            uiDBTween.TweenProperty(UI, "volume_db",-80,1);

            uiDBTween.SetPauseMode(Tween.TweenPauseMode.Process);

        }

        public void ResumeUI()
        {
            uiDBTween?.Kill();
            uiDBTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out);

            uiDBTween.TweenProperty(UI, "volume_db", musicDB, 1);
            uiDBTween.SetPauseMode(Tween.TweenPauseMode.Process);


        }




        //##########################################################################
        //#########################/          BOSS          /#######################
        //##########################################################################


        public void PlayBossExplosion()
        {
            bossExplosion.Play();
        }

        public void PlayBossPreExplosion()
        {
            bossPreExplosion.Play();
        }

        public void PlayBossShoot()
        {
            bossShoot.Play();
        }




        //##########################################################################
        //#########################/       COLLECTABLE      /#######################
        //##########################################################################



        public void PlaySmartBombCollectable()
        {
            smartBombCollectable.Play();
        }
        public void PlayHealCollectable()
        {
            healCollectable.Play();
        }
        public void PlayLevelUpCollectable()
        {
            levelUpCollectable.Play();
        }





        //##########################################################################
        //#########################/          ENEMY         /#######################
        //##########################################################################



        public void PlayEnemyOneExplosion()
        {
            enemyOneExplosionList[GD.RandRange(0, enemyOneExplosionList.Count-1)].Play();
        }

        public void PlayEnemyShoot()
        {
            enemyShoot.Play();
        }


        public void PlayEnemyExplosion()
        {
            enemyExplosion.Play();
        }




        //##########################################################################
        //#########################/         OTHER          /#######################
        //##########################################################################



        public void PlayObstacleDestruction()
        {
            obstacleDestruction.Play();
        }

        public void PlaySmartBomb()
        {
            bomb.Play();
        }

        public void PlayConstruction()
        {
            construction.Play();
        }

        //##########################################################################
        //#########################/         PLAYER         /#######################
        //##########################################################################



        public void PlayExplosion()
        {
            playerExplosion.Play();
        }

        public void PlayPlayerSHoot()
        {
            playerShootList[GD.RandRange(0, enemyOneExplosionList.Count - 1)].Play();
        }


        public void PlayColorSwap()
        {
            colorSwap.Play();
        }

        public void PlayCamouflageIn()
        {
            camouflage_IN.Play();
        }

        public void PlayCamouflageOut()
        {
            camouflage_OUT.Play();
        }

        public void PlayLoseLife()
        {
            loselife.Play();
        }




        //##########################################################################
        //#########################/           UI           /#######################
        //##########################################################################


        public void PlayWinJingle()
        {
            winJingle.Play();
        }

        public void PlayLoseJingle()
        {
            gameOverJingle.Play();
        }

        public void PlayUIClick()
        {
            uiClick.Play();
        }

        public void PlayUIStart()
        {
            uiStart.Play();
        }

        public void PlayUIPause()
        {
            uiPause.Play();
        }

    }
}
