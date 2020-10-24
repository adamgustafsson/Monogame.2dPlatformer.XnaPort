using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Model class for handeling of all enemy logic
    /// </summary>
    class EnemyLogic
    {
        //Variables
        private TMXLevel m_level;
        private Player m_player;
        private List<Enemy> m_enemies;
        private List<Projectile> m_enemyProjectiles;

        //Constructor
        public EnemyLogic(TMXLevel m_TMXLevel, Player a_player)
        {
            this.m_level = m_TMXLevel;
            this.m_player = a_player;
            this.m_enemies = new List<Enemy>();
            this.m_enemyProjectiles = new List<Projectile>();
            LoadEnemies();
        }

        //Loading enemy mapobject from the level
        private void LoadEnemies()
        {
            if (m_level.GetEnemieObjects().Any())
            {
                foreach (MapObject enemyObj in m_level.GetEnemieObjects())
                    m_enemies.Add(new Enemy(enemyObj, (Enemy.Type)Convert.ToInt32(enemyObj.Type)));
            }
        }

        //Updating enemies
        internal void Update(float a_elapsedTime)
        {
            m_enemyProjectiles.RemoveAll(x => x.Active == false);

            foreach(Enemy enemy in m_enemies)
            {
                if (!m_player.IsAlive)
                    enemy.IsAlive = true;

                //Fall & jump mechanics
                //Needs to be updated before Vertical collision
                if (!enemy.IsOnGround && !enemy.IsJumping)
                    enemy.Obj.Bounds.Y += Convert.ToInt32(a_elapsedTime * enemy.Speed.X);

                if ((enemy.EnemyType == Enemy.Type.Jumper || enemy.EnemyType == Enemy.Type.Shooter) && enemy.IsAlive)
                {
                    if (enemy.IsJumping)
                    {
                        enemy.IsOnGround = false;
                        enemy.Obj.Bounds.Y -= (int)enemy.Speed.Y;
                        enemy.SpeedY -= a_elapsedTime / 0.10f;
                    }
                    //Updating jump mechanics within the enemy object
                    enemy.Update(a_elapsedTime);
                }

                //Collision handeling; Vertical
                m_level.CheckVerticalCollision(enemy);

                if (enemy.IsAlive)
                {
                    enemy.Visibility = 1;

                    //Updating logic for enemy type; Shooter
                    if (enemy.EnemyType == Enemy.Type.Shooter)
                    {
                        #region Shooter
                            bool leftDirection = m_player.Rectangle.Center.X < enemy.Rectangle.Center.X;

                            if (enemy.ProjectileCoolDown <= 0)
                            {
                                int shotSpawn;
                                if (leftDirection)
                                    shotSpawn = enemy.Rectangle.Left;
                                else
                                    shotSpawn = enemy.Rectangle.Right;

                                m_enemyProjectiles.Add(new Projectile(shotSpawn, enemy.Rectangle.Center.Y - 5, leftDirection));
                                enemy.ProjectileCoolDown = 2;
                            }
                            else
                                enemy.ProjectileCoolDown -= a_elapsedTime;

                            if ((enemy.ProjectileCoolDown < 0.5 || enemy.ProjectileCoolDown > 1.5))
                            {
                                if (leftDirection)
                                    enemy.UnitState = State.ID.JUMPING_LEFT;
                                else
                                    enemy.UnitState = State.ID.JUMPING_RIGHT;
                            }
                            else
                            {
                                if (leftDirection)
                                    enemy.UnitState = State.ID.FACING_LEFT;
                                else
                                    enemy.UnitState = State.ID.FACING_RIGHT;
                            }
                            #endregion
                    }

                    //Updating logic for enemy type; Crawler and Jumper
                    #region Crawler & Jumper horizontal movments
                        if ((enemy.UnitState != State.ID.MOVING_RIGHT && enemy.UnitState != State.ID.MOVING_LEFT) && enemy.EnemyType != Enemy.Type.Shooter)
                            enemy.UnitState = State.ID.MOVING_RIGHT;
                        if (enemy.UnitState == State.ID.MOVING_RIGHT)
                            enemy.Obj.Bounds.X += Convert.ToInt32(a_elapsedTime * enemy.Speed.X);
                        else if (enemy.UnitState == State.ID.MOVING_LEFT)
                            enemy.Obj.Bounds.X -= Convert.ToInt32(a_elapsedTime * enemy.Speed.X);
                    #endregion

                    if (m_level.DidReachTurningPoint(enemy))
                    {
                        if (enemy.UnitState == State.ID.MOVING_RIGHT)
                            enemy.UnitState = State.ID.MOVING_LEFT;
                        else
                            enemy.UnitState = State.ID.MOVING_RIGHT;
                    }
                    CheckPlayerCollision(enemy);
                }
                else
                    enemy.Visibility = enemy.Visibility - 0.01f; 
        }

        //Updating all projectiles generated from enemy type; Shooter
        foreach (Projectile projectile in m_enemyProjectiles)
        {
            if (projectile.DirectionLeft)
                projectile.X -= Convert.ToInt32(a_elapsedTime * projectile.Speed);
            else
                projectile.X += Convert.ToInt32(a_elapsedTime * projectile.Speed);
            if (projectile.Rec.Intersects(m_player.Rectangle))
                m_player.LostLife();
        }
    }

        //Checks whether the player successfully jumps on an enemy
        private void CheckPlayerCollision(Enemy enemy)
        {
            if(m_player.Rectangle.Intersects(enemy.Obj.Bounds))
            {
                if ((m_player.Rectangle.Bottom > enemy.Rectangle.Top) && (m_player.IsJumping && m_player.SpeedY < 0) || m_player.IsFalling)
                {
                    enemy.IsJumping = false;
                    enemy.IsAlive = false;
                }
                else
                    m_player.LostLife();
            }
        }

        //Getters
        internal List<Projectile> EnemyProjectiles
        {
            get { return m_enemyProjectiles; }
        }
        internal List<Enemy> Enemies
        {
            get { return m_enemies; }
        }
    }
}
