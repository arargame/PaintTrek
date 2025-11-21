using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class GlobalTexture
    {
        public static Texture2D astreoidTexture;
        public static Texture2D headBone1Texture;
        public static Texture2D headBone2Texture;
        public static Texture2D longBoneTexture;
        public static Texture2D bristleTexture;
        public static Texture2D cacaoTexture;
        public static Texture2D childTrilobitTexture;
        public static Texture2D childTrilobitEggTexture;
        public static Texture2D cometTexture;
        public static Texture2D eyeTexture;
        public static Texture2D hearthBreakerTexture;
        public static Texture2D Invader1Texture;
        public static Texture2D Invader2Texture;
        public static Texture2D Invader3Texture;
        public static Texture2D jellyFishTexture;
        public static Texture2D monsterFishTexture;
        public static Texture2D mrBrainTexture;
        public static Texture2D sharpCubeTexture;
        public static Texture2D snakeStoneTexture;
        public static Texture2D spaceSnakeTexture;
        public static Texture2D ufo1Texture;
        public static Texture2D ufo2Texture;

        public static Texture2D basicEnemyBulletTexture;
        public static Texture2D boss10FireTexture;
        public static Texture2D bossFire1Texture;
        public static Texture2D starFireTexture;
        public static Texture2D ufo2FireTexture;
        public static Texture2D ufoLaserTexture;
        public static Texture2D bouncingFireTexture;
        public static Texture2D laserTexture;
        public static Texture2D orbitalFireTexture;
        public static Texture2D rocketTexture;
        public static Texture2D explosionTexture;
        public static Texture2D waveGunTexture;

        public static Texture2D boss1Texture;
        public static Texture2D boss2Texture;
        public static Texture2D boss3Texture;
        public static Texture2D snakeStoneUpTexture;
        public static Texture2D snakeStoneDownTexture;
        public static Texture2D snakeStoneLeftTexture;
        public static Texture2D boss5Texture;
        public static Texture2D boss6Texture;
        public static Texture2D boss7Texture;
        public static Texture2D boss8Texture;
        public static Texture2D boss9Texture;
        public static Texture2D boss10Texture;

        public static Texture2D bouncingFireCollectionTexture;
        public static Texture2D bubbleTexture;
        public static Texture2D blackDiamondTexture;
        public static Texture2D greenDiamondTexture;
        public static Texture2D redDiamondTexture;
        public static Texture2D blueDiamondTexture;
        public static Texture2D diffusedFireSupplyTexture;
        public static Texture2D orbitalFireSupplyTexture;
        public static Texture2D pixelSupplyTexture;
        public static Texture2D rocketSupplyTexture;
        public static Texture2D tripleFireSupplyTexture;
        public static Texture2D wrenchTexture;
        public static Texture2D waveGunSupplyTexture;

        public static void LoadTextures() 
        {
            astreoidTexture = Globals.Content.Load<Texture2D>("Sprites/Target/asteroidSpriteSheet");
            headBone1Texture = Globals.Content.Load<Texture2D>("Sprites/Target/headBone1");
            headBone2Texture = Globals.Content.Load<Texture2D>("Sprites/Target/headBone2");
            longBoneTexture = Globals.Content.Load<Texture2D>("Sprites/Target/longBone");
            bristleTexture = Globals.Content.Load<Texture2D>("Sprites/Target/bristleSpriteSheet");
            cacaoTexture = Globals.Content.Load<Texture2D>("Sprites/Target/cacaoSpriteSheet");
            childTrilobitTexture = Globals.Content.Load<Texture2D>("Sprites/Target/trilobitChildSpriteSheet");
            childTrilobitEggTexture = Globals.Content.Load<Texture2D>("Sprites/Target/childTrilobitEggSpriteSheet");
            cometTexture=Globals.Content.Load<Texture2D>("Sprites/Target/cometSpriteSheet");
            eyeTexture = Globals.Content.Load<Texture2D>("Sprites/Target/eyeSpriteSheet");
            hearthBreakerTexture=Globals.Content.Load<Texture2D>("Sprites/Target/heartBreakerSpriteSheet");
            Invader1Texture = Globals.Content.Load<Texture2D>("Sprites/Target/invader1SpriteSheet");
            Invader2Texture = Globals.Content.Load<Texture2D>("Sprites/Target/invader2SpriteSheet");
            Invader3Texture = Globals.Content.Load<Texture2D>("Sprites/Target/invader3SpriteSheet");
            jellyFishTexture = Globals.Content.Load<Texture2D>("Sprites/Target/squidSpriteSheet");
            monsterFishTexture = Globals.Content.Load<Texture2D>("Sprites/Target/monsterFishSpriteSheet");
            mrBrainTexture = Globals.Content.Load<Texture2D>("Sprites/Target/mrBrainSpriteSheet");
            sharpCubeTexture = Globals.Content.Load<Texture2D>("Sprites/Target/cubeSpriteSheet");
            snakeStoneTexture = Globals.Content.Load<Texture2D>("Sprites/Target/snakeStone");
            spaceSnakeTexture = Globals.Content.Load<Texture2D>("Sprites/Target/spaceSnakeSpriteSheet");
            ufo1Texture = Globals.Content.Load<Texture2D>("Sprites/Target/ufoSpriteSheet");
            ufo2Texture = Globals.Content.Load<Texture2D>("Sprites/Target/ufo2SpriteSheet");

            basicEnemyBulletTexture = Globals.Content.Load<Texture2D>("Guns/basicEnemyBullet");
            boss10FireTexture = Globals.Content.Load<Texture2D>("Guns/boss10Fire");
            bossFire1Texture = Globals.Content.Load<Texture2D>("Guns/bossFire1");
            starFireTexture = Globals.Content.Load<Texture2D>("Guns/starFireSpriteSheet");
            ufo2FireTexture = Globals.Content.Load<Texture2D>("Guns/ufo2Fire");
            ufoLaserTexture = Globals.Content.Load<Texture2D>("Guns/ufoLaser");
            bouncingFireTexture = Globals.Content.Load<Texture2D>("Guns/bouncingFireSpriteSheet");
            laserTexture = Globals.Content.Load<Texture2D>("Guns/laser");
            orbitalFireTexture = Globals.Content.Load<Texture2D>("Guns/orbitalFireSpriteSheet");
            rocketTexture = Globals.Content.Load<Texture2D>("Guns/rocketSpriteSheet");
            explosionTexture = Globals.Content.Load<Texture2D>("Explosions/explosion");
            waveGunTexture = Globals.Content.Load<Texture2D>("Guns/waveGunSpriteSheet");
            

            boss1Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss1SpriteSheet");
            boss2Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss2SpriteSheet");
            boss3Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss3SpriteSheet");
            snakeStoneUpTexture = Globals.Content.Load<Texture2D>("Sprites/Boss/snakeStoneSpriteSheetUp"); ;
            snakeStoneDownTexture = Globals.Content.Load<Texture2D>("Sprites/Boss/snakeStoneSpriteSheetDown"); ;
            snakeStoneLeftTexture = Globals.Content.Load<Texture2D>("Sprites/Boss/snakeStoneSpriteSheetLeft"); ;
            boss5Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss5SpriteSheet");
            boss6Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss6SpriteSheet");
            boss7Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss7SpriteSheet");
            boss8Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss8SpriteSheet");
            boss9Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss9SpriteSheet");
            boss10Texture = Globals.Content.Load<Texture2D>("Sprites/Boss/boss10SpriteSheet");

            bouncingFireCollectionTexture = Globals.Content.Load<Texture2D>("CollectableObjects/bouncingBallSupply");
            bubbleTexture = Globals.Content.Load<Texture2D>("CollectableObjects/bubble");
            blackDiamondTexture = Globals.Content.Load<Texture2D>("CollectableObjects/blackDiamond");
            redDiamondTexture = Globals.Content.Load<Texture2D>("CollectableObjects/redDiamond");
            greenDiamondTexture = Globals.Content.Load<Texture2D>("CollectableObjects/greenDiamond");
            blueDiamondTexture = Globals.Content.Load<Texture2D>("CollectableObjects/blueDiamond");
            diffusedFireSupplyTexture = Globals.Content.Load<Texture2D>("CollectableObjects/diffusedFireSupply");
            orbitalFireSupplyTexture = Globals.Content.Load<Texture2D>("CollectableObjects/orbitalFireSupply");
            pixelSupplyTexture = Globals.Content.Load<Texture2D>("CollectableObjects/pixelSupply");
            rocketSupplyTexture = Globals.Content.Load<Texture2D>("CollectableObjects/rocketSupply");
            tripleFireSupplyTexture = Globals.Content.Load<Texture2D>("CollectableObjects/tripleFireSupply");
            wrenchTexture = Globals.Content.Load<Texture2D>("CollectableObjects/wrenchSpriteSheet");
            waveGunSupplyTexture = Globals.Content.Load<Texture2D>("CollectableObjects/waveGunSupply");
        }
    }
}
