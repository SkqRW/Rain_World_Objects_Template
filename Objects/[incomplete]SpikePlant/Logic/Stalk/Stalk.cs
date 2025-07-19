using System;
using MoreSlugcats;
namespace SpikePlant;

//Strongly inspired in the dangleFruit logic
public partial class SpikePlant : Weapon, IPlayerEdible
{
    //This is a copy of the FruitTemplate of the Happy Astronaut (anubisrah), Thanks for the help :D
    //This get a rework when the conceptual art is done.
    public class Stalk : UpdatableAndDeletable, IDrawable
    {
        public SpikePlant fruit;

        public Vector2 stuckPos;

        public float ropeLength;

        public Vector2[] displacements;

        public Vector2[,] segs;

        public int releaseCounter;

        protected float connRad;


        public Stalk(SpikePlant fruit, Room room, Vector2 fruitPos)
        {
            this.fruit = fruit;
            fruit.firstChunk.HardSetPosition(fruitPos);
            stuckPos.x = fruitPos.x;
            ropeLength = -1f;
            int x = room.GetTilePosition(fruitPos).x;
            for (int i = room.GetTilePosition(fruitPos).y; i < room.TileHeight; i++)
            {
                if (room.GetTile(x, i).Solid)
                {
                    stuckPos.y = room.MiddleOfTile(x, i).y - 10f;
                    ropeLength = Mathf.Abs(stuckPos.y - fruitPos.y);
                    break;
                }
            }
            segs = new Vector2[Math.Max(1, (int)(ropeLength / 15f)), 3];
            for (int j = 0; j < segs.GetLength(0); j++)
            {
                float t = (float)j / (float)(segs.GetLength(0) - 1);
                segs[j, 0] = Vector2.Lerp(stuckPos, fruitPos, t);
                segs[j, 1] = segs[j, 0];
            }
            connRad = ropeLength / Mathf.Pow(segs.GetLength(0), 1.1f);
            displacements = new Vector2[segs.GetLength(0)];
            UnityEngine.Random.State state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(fruit.abstractPhysicalObject.ID.RandomSeed);
            for (int k = 0; k < displacements.Length; k++)
            {
                //This controls how squiggly the stalk is. I multiplied it by 0.5 to make it less dramatic
                displacements[k] = Custom.RNV() * new Vector2(0.5f, 0.5f);
            }
            UnityEngine.Random.state = state;
        }
        public override void Update(bool eu)
        {
            base.Update(eu);
            if (ropeLength == -1f)
            {
                Destroy();
                return;
            }
            ConnectSegments(dir: true);
            ConnectSegments(dir: false);
            for (int i = 0; i < segs.GetLength(0); i++)
            {
                segs[i, 1] = segs[i, 0];
                segs[i, 0] += segs[i, 2];
                segs[i, 2] *= 0.99f;
                segs[i, 2].y -= 0.9f;
            }
            ConnectSegments(dir: false);
            ConnectSegments(dir: true);
            if (releaseCounter > 0)
            {
                releaseCounter--;
            }
            if (fruit != null)
            {
                fruit.setRotation = Custom.DirVec(fruit.firstChunk.pos, segs[segs.GetLength(0) - 1, 0]);
                if (!Custom.DistLess(fruit.firstChunk.pos, stuckPos, ropeLength * 1.4f + 10f) || fruit.slatedForDeletetion || fruit.bites < 1 || fruit.room != room || releaseCounter == 1)
                {
                    fruit.AbstrConsumable.Consume();
                    fruit = null;
                }
            }
        }

        private void ConnectSegments(bool dir)
        {
            int num = ((!dir) ? (segs.GetLength(0) - 1) : 0);
            bool flag = false;
            while (!flag)
            {
                if (num == 0)
                {
                    if (!Custom.DistLess(segs[num, 0], stuckPos, connRad))
                    {
                        Vector2 vector = Custom.DirVec(segs[num, 0], stuckPos) * (Vector2.Distance(segs[num, 0], stuckPos) - connRad);
                        segs[num, 0] += vector;
                        segs[num, 2] += vector;
                    }
                }
                else
                {
                    if (!Custom.DistLess(segs[num, 0], segs[num - 1, 0], connRad))
                    {
                        Vector2 vector2 = Custom.DirVec(segs[num, 0], segs[num - 1, 0]) * (Vector2.Distance(segs[num, 0], segs[num - 1, 0]) - connRad);
                        segs[num, 0] += vector2 * 0.5f;
                        segs[num, 2] += vector2 * 0.5f;
                        segs[num - 1, 0] -= vector2 * 0.5f;
                        segs[num - 1, 2] -= vector2 * 0.5f;
                    }
                    if (num == segs.GetLength(0) - 1 && fruit != null && !Custom.DistLess(segs[num, 0], fruit.firstChunk.pos, connRad))
                    {
                        Vector2 vector3 = Custom.DirVec(segs[num, 0], fruit.firstChunk.pos) * (Vector2.Distance(segs[num, 0], fruit.firstChunk.pos) - connRad);
                        segs[num, 0] += vector3 * 0.75f;
                        segs[num, 2] += vector3 * 0.75f;
                        fruit.firstChunk.vel -= vector3 * 0.25f;
                    }
                }
                num += (dir ? 1 : (-1));
                if (dir && num >= segs.GetLength(0))
                {
                    flag = true;
                }
                else if (!dir && num < 0)
                {
                    flag = true;
                }
            }
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = TriangleMesh.MakeLongMesh(segs.GetLength(0), pointyTip: false, customColor: false);
            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = stuckPos;
            float num = 1.5f;
            for (int i = 0; i < segs.GetLength(0); i++)
            {
                float num2 = (float)i / (float)(segs.GetLength(0) - 1);
                float num3 = Custom.LerpMap(num2, 0f, 0.5f, 1f, 0f) + Mathf.Lerp(1f, 0.5f, Mathf.Sin(Mathf.Pow(num2, 3.5f) * (float)Math.PI));
                Vector2 vector2 = Vector2.Lerp(segs[i, 1], segs[i, 0], timeStacker);
                if (i == segs.GetLength(0) - 1 && fruit != null)
                {
                    vector2 = Vector2.Lerp(fruit.firstChunk.lastPos, fruit.firstChunk.pos, timeStacker);
                }
                Vector2 normalized = (vector - vector2).normalized;
                Vector2 vector3 = Custom.PerpendicularVector(normalized);
                if (i < segs.GetLength(0) - 1)
                {
                    vector2 += (normalized * displacements[i].y + vector3 * displacements[i].x) * Custom.LerpMap(Vector2.Distance(vector, vector2), connRad, connRad * 5f, 4f, 0f);
                }
                vector2 = new Vector2(Mathf.Floor(vector2.x) + 0.5f, Mathf.Floor(vector2.y) + 0.5f);
                (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4, vector - vector3 * num - camPos);
                (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 1, vector + vector3 * num - camPos);
                (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 2, vector2 - vector3 * num3 - camPos);
                (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 3, vector2 + vector3 * num3 - camPos);
                vector = vector2;
                num = num3;
            }
            if (base.slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = palette.blackColor;
            }
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].RemoveFromContainer();
                rCam.ReturnFContainer("Background").AddChild(sLeaser.sprites[i]);
            }
        }
    }
}

