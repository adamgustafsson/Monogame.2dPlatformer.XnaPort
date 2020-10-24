using FuncWorks.XNA.XTiled;

using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// GameModel
    /// Main model class for game logics
    /// </summary>
    class GameModel
    {
        //Variables
        private TMXLevel m_TMXLevel;
        private Map m_currentMap;
        private Player m_player;
        private PlayerLogic m_playerLogistics;
        private EnemyLogic m_enemyLogistics;

        //Constructor
        public GameModel(List<Map> a_TMXMaps)
        {
            this.m_TMXLevel = new TMXLevel(a_TMXMaps);
            this.m_currentMap = m_TMXLevel.CurrentMap;
            this.m_player = new Player(m_TMXLevel.PlayerObject);
            this.m_playerLogistics = new PlayerLogic(m_TMXLevel, m_player);
            this.m_enemyLogistics = new EnemyLogic(m_TMXLevel, m_player);
        }

        //Updating game logic
        internal void UpdateSimulation(float a_elapsedTime)
        {
            m_TMXLevel.Update(a_elapsedTime);
            m_playerLogistics.Update(a_elapsedTime);
            if (!m_TMXLevel.TriggerOneIsActive)
                m_enemyLogistics.Update(a_elapsedTime);
        }

        #region Getters

            internal TMXLevel TMXLevel
            {
                get { return m_TMXLevel; }
            }
            internal Map CurrentMap
            {
                get { return m_currentMap; }
            }
            internal Player Player
            {
                get { return m_player; }
            }
            internal List<Projectile> Projectiles
            {
                get { return m_enemyLogistics.EnemyProjectiles; }
            }
            internal List<Enemy> Enemies
            {
                get { return m_enemyLogistics.Enemies; }
            }

        #endregion

    }
}
