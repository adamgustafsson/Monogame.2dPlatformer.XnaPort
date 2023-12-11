using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Main class for handling all objects accosiated with a .tmx map
    /// </summary>
    class TMXLevel
    {

        #region Variables
        private List<Map> m_mapList;
        private List<MovingPlatform> m_movingPlatforms;
        private int m_levelIndex;
        private Stopwatch m_triggerOneWatch;
        private Stopwatch m_triggerTwoWatch;

        //Object layer indexes
        public int BackgroundIndex;
        public int ForegroundIndex;
        public int TriggerIndex;
        public int CollisionIndex;
        public int DeathIndex;
        public int PlayerIndex;
        public int EnemiesIndex;
        public int MovementAidIndex;
        public int ItemsAndTriggersIndex;

        //Objectlayers
        private ObjectLayer m_backgroundLayer;
        private ObjectLayer m_foregroundLayer;
        private ObjectLayer m_triggerLayer;
        private ObjectLayer m_collisionLayer;
        private ObjectLayer m_deathLayer;
        private ObjectLayer m_playerLayer;
        private ObjectLayer m_enemyLayer;
        private ObjectLayer m_movementAidLayer;
        private ObjectLayer m_itemsAndTriggersLayer; 
        #endregion

        //Enum for item types
        internal enum ItemType { Coin = 1, ExtraLife = 2, PowerUpOne = 3, TriggerOne = 4, TriggerTwo = 5 };
        //Enum for movement aid types
        internal enum MovementAidType { Ladder = 1, Trampoline = 2};

        //Constructor
        public TMXLevel(List<Map> a_TMXMaps)
        {
            this.m_mapList = a_TMXMaps;
            this.m_movingPlatforms = new List<MovingPlatform>();
            this.m_triggerOneWatch = new Stopwatch();
            this.m_triggerTwoWatch = new Stopwatch();

            AssignObjectLayerIndexes();
            AssignTileLayerIndexes();
            AssignObjectLayers();

            InitializeMovingPlatforms();
        }
       
        //Assigning index values to all specified Object layers
        private void AssignObjectLayerIndexes()
        {
            for (int i = 0; i < m_mapList[LevelIndex].ObjectLayers.Count; i++)
            {
                switch (m_mapList[LevelIndex].ObjectLayers[i].Name)
                {
                    case "Death":
                        DeathIndex = i;
                        break;
                    case "Collision":
                        CollisionIndex = i;
                        break;
                    case "Player":
                        PlayerIndex = i;
                        break;
                    case "Enemies":
                        EnemiesIndex = i;
                        break;
                    case "MovementAid":
                        MovementAidIndex = i;
                        break;
                    case "ItemsAndTriggers":
                        ItemsAndTriggersIndex = i;
                        break;
                }
            }
        }

        //Assigning index values to all specified Tile layers
        private void AssignTileLayerIndexes()
        {
            for (int i = 0; i < m_mapList[LevelIndex].TileLayers.Count; i++)
            {
                switch (m_mapList[LevelIndex].TileLayers[i].Name)
                {
                    case "Background":
                        BackgroundIndex = i;
                        break;
                    case "Foreground":
                        ForegroundIndex = i;
                        break;
                    case "Trigger":
                        TriggerIndex = i;
                        break;
                }
            }
        }

        //Initialize Object & Tile layer variables
        private void AssignObjectLayers()
        {
            m_backgroundLayer = m_mapList[LevelIndex].ObjectLayers[BackgroundIndex];
            m_foregroundLayer = m_mapList[LevelIndex].ObjectLayers[ForegroundIndex];
            m_triggerLayer = m_mapList[LevelIndex].ObjectLayers[TriggerIndex];
            m_collisionLayer = m_mapList[LevelIndex].ObjectLayers[CollisionIndex];
            m_deathLayer = m_mapList[LevelIndex].ObjectLayers[DeathIndex];
            m_playerLayer = m_mapList[LevelIndex].ObjectLayers[PlayerIndex];
            m_enemyLayer = m_mapList[LevelIndex].ObjectLayers[EnemiesIndex];
            m_movementAidLayer = m_mapList[LevelIndex].ObjectLayers[MovementAidIndex];
            m_itemsAndTriggersLayer = m_mapList[LevelIndex].ObjectLayers[ItemsAndTriggersIndex];
        }

        //Initializing & collecting all the platforms from the current level
        private void InitializeMovingPlatforms()
        {
            //Collecting all mapobjects that are moving platforms
            List<MapObject> movingPlatForms = new List<MapObject>();
            foreach (MapObject obj in m_collisionLayer.MapObjects)
            {
                if (obj.Name != null && obj.Name != "")
                    movingPlatForms.Add(obj);
            }

            var platforms = movingPlatForms.Select(x => x.Name).Distinct();            

            foreach (var platform in platforms)
            {

                MovingPlatform mp = new MovingPlatform(m_collisionLayer.MapObjects.Where(x => x.Name == platform && x.Polyline != null).First(),
                                                        m_collisionLayer.MapObjects.Where(x => x.Name == platform && !x.TileID.HasValue && x.Polyline == null).First());
                m_movingPlatforms.Add(mp);
            }  
        }

        //Return a list of enemy map objects
        internal List<MapObject> GetEnemieObjects()
        {
            List<MapObject> enemyObjects = new List<MapObject>();
            foreach (MapObject enemy in m_enemyLayer.MapObjects)
            {
                enemyObjects.Add(enemy);
            }
            return enemyObjects;
        }

        //Additional collision handeling for moving platforms
        internal void CheckMovingPlatformCollision(MapObject a_mapObj)
        {
            foreach (MovingPlatform obj in m_movingPlatforms)
            {
                Rectangle area = obj.GetPlatformArea;
                area.Inflate(0, 10);

                if (area.Intersects(a_mapObj.Bounds))
                    a_mapObj.Bounds.Offset(obj.Change);
            }
        }

        //Horizontinal collision handeling
        internal void CheckHorizontalCollision(Unit a_unit)
        {
            if (a_unit.LastLocation.X != a_unit.Obj.Bounds.X || (a_unit.IsOnTemporaryPlatform))
            {
                //Increase the player collision rectangle size in order to controll if the player is on the ground
                Rectangle playerRegion = a_unit.Obj.Bounds;
                playerRegion.Inflate(10, 10);

                foreach (var obj in CurrentMap.GetObjectsInRegion(CollisionIndex, playerRegion).Where(x => x.Polyline == null))
                {
                    if (obj.Type == "Trigger")
                        a_unit.IsOnTemporaryPlatform = true;
                    else
                        a_unit.IsOnTemporaryPlatform = false;

                    if (String.IsNullOrWhiteSpace(obj.Type) || (obj.Type == "Trigger" && TriggerTwoIsActive))
                    {
                        AdjustLocation(a_unit.Obj, obj, true);
                    }
                    //Checking wheter the player is still on the ground
                    if ((obj.Bounds.Top - 3 > a_unit.Obj.Bounds.Bottom || a_unit.Obj.Bounds.Left > (obj.Bounds.X + obj.Bounds.Width) || a_unit.Obj.Bounds.Right < obj.Bounds.X))
                    {
                        a_unit.IsOnGround = false;
                    }
                    else if (obj.Type == "Trigger" && !TriggerTwoIsActive)
                    {
                        a_unit.IsOnGround = false;
                    }
                }
            }
        }

        //Vertical collision handeling
        internal void CheckVerticalCollision(Unit a_unit)
        {
            if (a_unit.LastLocation.Y != a_unit.Obj.Bounds.Location.Y)
            {
                foreach (var obj in CurrentMap.GetObjectsInRegion(CollisionIndex, a_unit.Obj.Bounds).Where(x => x.Polyline == null))
                {
                    if (String.IsNullOrWhiteSpace(obj.Type) || (obj.Type == "Trigger" && TriggerTwoIsActive))
                    {
                        if (a_unit.LastLocation.Y < a_unit.Obj.Bounds.Y && AdjustLocation(a_unit.Obj, obj, false))
                        {
                            a_unit.IsOnGround = true;
                            a_unit.IsJumping = false;
                            a_unit.SpeedY = 6f;
                        }
                        else if (AdjustLocation(a_unit.Obj, obj, false))
                        {
                            a_unit.DidHitRoof = true;
                            a_unit.SpeedY = 0f;
                        }
                    }
                }
            } 
        }

        //Obstacle/deathlayer collision handeling
        internal void CheckObstaclesCollision(Unit a_unit)
        {
            foreach (var obj in CurrentMap.GetObjectsInRegion(DeathIndex, a_unit.Obj.Bounds))
                a_unit.IsAlive = false;
        }

        //Checks if the player interacts with a collectable item
        internal void CheckTriggersAndItems(Player a_player)
        {
            foreach (var obj in CurrentMap.GetObjectsInRegion(ItemsAndTriggersIndex, a_player.Rectangle))
            {
                if ((ItemType)Convert.ToInt32(obj.Type) == ItemType.Coin && obj.Visible)
                {
                    a_player.Coins++;
                    obj.Visible = false;
                }
                if ((ItemType)Convert.ToInt32(obj.Type) == ItemType.ExtraLife && obj.Visible)
                {
                    a_player.Life++;
                    obj.Visible = false;
                }
                if ((ItemType)Convert.ToInt32(obj.Type) == ItemType.PowerUpOne && obj.Visible)
                {
                    a_player.PowerUpOneIsActive = true;
                    //obj.Visible = false;
                }

                if ((ItemType)Convert.ToInt32(obj.Type) == ItemType.TriggerOne && !TriggerOneIsActive)
                {
                    TriggerOneIsActive = true;
                    m_triggerOneWatch.Start();
                }

                if ((ItemType)Convert.ToInt32(obj.Type) == ItemType.TriggerTwo && !TriggerTwoIsActive)
                {
                    TriggerTwoIsActive = true;
                    m_triggerTwoWatch.Start();
                }
            }
        }

        //Reset the maps collectable items
        internal void ResetTriggersAndItems()
        {
            foreach (MapObject item in m_itemsAndTriggersLayer.MapObjects)
                item.Visible = true;
        }

        //MovementAid interaction, Ladder
        internal bool UnitIsOnLadder(Unit a_unit)
        {
            Rectangle unitArea = a_unit.Rectangle;
            unitArea.Inflate(-10,0);

            foreach (var obj in CurrentMap.GetObjectsInRegion(MovementAidIndex, unitArea).Where(x => x.Type == "1"))
            {
                return true;
            }
            return false;
        }

        //MovementAid interaction, Trampoline
        internal bool UnitTriggeredTrampoline(Unit a_unit)
        {
            foreach (var obj in CurrentMap.GetObjectsInRegion(MovementAidIndex, a_unit.Rectangle).Where(x => x.Type == "2"))
            {
                if ((a_unit.Rectangle.Bottom > obj.Bounds.Top) && (a_unit.IsJumping && a_unit.SpeedY < 0) || a_unit.IsFalling)
                {
                    return true;
                }
            }
            return false;
        }

        //Checks if a enemy reaches a turning point; cliff or horizontal tile
        internal Boolean DidReachTurningPoint(Enemy a_enemy)
        {
            Rectangle collisionArea = a_enemy.Rectangle;
            collisionArea.Inflate(0,5);

            foreach (var obj in CurrentMap.GetObjectsInRegion(CollisionIndex, collisionArea))
            {
                if ((a_enemy.MaxRightLocation == 0 && a_enemy.MaxLeftLocation == 0)
                    && obj.Bounds.Top >= a_enemy.Rectangle.Bottom)
                {
                    a_enemy.MaxRightLocation = obj.Bounds.Right;
                    a_enemy.MaxLeftLocation = obj.Bounds.Left;
                }
                if (obj.Bounds.Top < a_enemy.Rectangle.Bottom)
                    return true;
            }
            if (a_enemy.MaxRightLocation <= a_enemy.Rectangle.Right)
                return true;
            if (a_enemy.MaxLeftLocation >= a_enemy.Rectangle.Left)
                return true;

            return false;
        }

        //Updating moving map objects
        internal void Update(float a_elapsedTime)
        {
            if(!TriggerOneIsActive)
            {
                foreach (MovingPlatform mp in m_movingPlatforms)
                    mp.Update(a_elapsedTime);
            }

            if (m_triggerOneWatch.ElapsedMilliseconds > 5000)
            {
                m_triggerOneWatch.Stop();
                m_triggerOneWatch.Reset();
                TriggerOneIsActive = false;
            }
            if (m_triggerTwoWatch.ElapsedMilliseconds > 5000)
            {
                m_triggerTwoWatch.Stop();
                m_triggerTwoWatch.Reset();
                TriggerTwoIsActive = false;
            }

        }

        //Handeling location adjustments @ collisions
        private Boolean AdjustLocation(MapObject a_unit, MapObject a_platform, Boolean a_Xlocation)
        {
            Rectangle delta = Rectangle.Intersect(a_unit.Bounds, a_platform.Bounds);

            if (a_Xlocation)
            {
                a_unit.Bounds.X += delta.Width * (delta.X > a_unit.Bounds.X ? -1 : 1);
                return delta.Width > 0;
            }
            else
            {
                a_unit.Bounds.Y += delta.Height * (delta.Y > a_unit.Bounds.Y ? -1 : 1);
                return delta.Height > 0;
            }
        }

        //Returning all map items and triggers in Vector4 format for a given region
        internal List<Vector4> GetItemsInRegion(Rectangle a_region)
        {
            List<Vector4> items = new List<Vector4>();

            foreach (var obj in CurrentMap.GetObjectsInRegion(ItemsAndTriggersIndex, a_region))
            {
                int isVisible = 0;
                int type = Convert.ToInt32(obj.Type);

                if (obj.Visible == true)
                    isVisible = 1;
                if ((ItemType)type == ItemType.PowerUpOne)
                {
                    obj.Bounds.Width = 8;
                    obj.Bounds.Height = 8;
                }

                items.Add(new Vector4(obj.Bounds.X, obj.Bounds.Y, type, isVisible));
            }

            return items;
        }

        //Returning all MovementAid objects in Vector4 format for a given region
        internal List<Vector4> GetMovmentAidsInRegion(Rectangle a_region)
        {
            List<Vector4> ma = new List<Vector4>();

            foreach (var obj in CurrentMap.GetObjectsInRegion(MovementAidIndex, a_region))
            {
                int isVisible = 0;
                int type = Convert.ToInt32(obj.Type);

                if (obj.Visible == true)
                    isVisible = 1;

                ma.Add(new Vector4(obj.Bounds.X, obj.Bounds.Y, type, isVisible));
            }

            return ma;
        }

        #region Get/Set

        internal Map CurrentMap
        {
            get { return m_mapList[m_levelIndex]; }
        }
        internal int LevelIndex
        {
            get { return m_levelIndex; }
            set { m_levelIndex = value; }
        }
        internal MapObject PlayerObject
        {
            get { return PlayerLayer.MapObjects[0]; }
        }
        
        internal bool TriggerOneIsActive { get; set; }
        internal bool TriggerTwoIsActive { get; set; }
        
        //Objectlayers Get/Set
        internal ObjectLayer BackgroundLayer
        {
            get { return m_backgroundLayer; }
            set { m_backgroundLayer = value; }
        }
        internal ObjectLayer ForegroundLayer
        {
            get { return m_foregroundLayer; }
            set { m_foregroundLayer = value; }
        }
        internal ObjectLayer CollisionLayer
        {
            get { return m_collisionLayer; }
            set { m_collisionLayer = value; }
        }
        internal ObjectLayer PlayerLayer
        {
            get { return m_playerLayer; }
            set { m_playerLayer = value; }
        }
        internal List<MovingPlatform> MovingPlatforms
        {
            get { return m_movingPlatforms; }
            set { m_movingPlatforms = value; }
        }

        #endregion

        //TODO: Refactor class due to size
    }


}

