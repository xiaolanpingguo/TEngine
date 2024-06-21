#if UNITY_EDITOR
using Lockstep.Framework;
using Lockstep.Game;
using UnityEngine;


namespace Lockstep.Framework
{
    // A node in a BoundsOctree
    // Copyright 2014 Nition, BSD licence (see LICENCE file). www.momentstudio.co.nz
    public partial class BoundsQuadTreeNode
    {
        /// <summary>
        /// Draws node boundaries visually for debugging.
        /// Must be called from OnDrawGizmos externally. See also: DrawAllObjects.
        /// </summary>
        /// <param name="depth">Used for recurcive calls to this method.</param>
        public void DrawBoundQuadTreeNode(float depth = 0)
        {
            float tintVal = depth / 7; // Will eventually get values > 1. Color rounds to 1 automatically
            Gizmos.color = new Color(tintVal, 0, 1.0f - tintVal);

            LRect thisBounds = CreateLRect(Center, new LVector2(adjLength, adjLength));

            LVector3 lv3cen = thisBounds.center.ToLVector3();
            Vector3 uv3cen = new Vector3(lv3cen.x.ToFloat(), lv3cen.y.ToFloat(), lv3cen.z.ToFloat());
            LVector3 lv3size = thisBounds.size.ToLVector3();
            Vector3 uv3size = new Vector3(lv3size.x.ToFloat(), lv3size.y.ToFloat(), lv3size.z.ToFloat());
            Gizmos.DrawWireCube(uv3cen, uv3size);

            if (children != null)
            {
                depth++;
                for (int i = 0; i < NUM_CHILDREN; i++)
                {
                    children[i].DrawBoundQuadTreeNode(depth);
                }
            }

            Gizmos.color = Color.white;
        }

        /// <summary>
        /// Draws the bounds of all objects in the tree visually for debugging.
        /// Must be called from OnDrawGizmos externally. See also: DrawAllBounds.
        /// </summary>
        public void DrawAllObjects()
        {
            float tintVal = (BaseLength / 20).ToFloat();
            Gizmos.color = new Color(0, 1.0f - tintVal, tintVal, 0.25f);

            foreach (OctreeObject obj in objects)
            {
                LVector3 lv3cen = obj.Bounds.center.ToLVector3();
                Vector3 uv3cen = new Vector3(lv3cen.x.ToFloat(), lv3cen.y.ToFloat(), lv3cen.z.ToFloat());
                LVector3 lv3size = obj.Bounds.size.ToLVector3();
                Vector3 uv3size = new Vector3(lv3size.x.ToFloat(), lv3size.y.ToFloat(), lv3size.z.ToFloat());
                Gizmos.DrawCube(uv3cen, uv3size);
            }

            if (children != null)
            {
                for (int i = 0; i < NUM_CHILDREN; i++)
                {
                    children[i].DrawAllObjects();
                }
            }

            Gizmos.color = Color.white;
        }
    }

    public partial class BoundsQuadTree
    {
        // Intended for debugging. Must be called from OnDrawGizmos externally
        // See also DrawAllBounds and DrawAllObjects
        /// <summary>
        /// Visualises collision checks from IsColliding and GetColliding.
        /// Collision visualisation code is automatically removed from builds so that collision checks aren't slowed down.
        /// </summary>
        public void DrawCollisionChecks()
        {
            int count = 0;
            foreach (LRect collisionCheck in lastBoundsCollisionChecks)
            {
                Gizmos.color = new Color(1.0f, 1.0f - ((float)count / numCollisionsToSave), 1.0f);
                LVector3 lv3cen = collisionCheck.center.ToLVector3();
                Vector3 uv3cen = new Vector3(lv3cen.x.ToFloat(), lv3cen.y.ToFloat(), lv3cen.z.ToFloat());
                LVector3 lv3size = collisionCheck.size.ToLVector3();
                Vector3 uv3size = new Vector3(lv3size.x.ToFloat(), lv3size.y.ToFloat(), lv3size.z.ToFloat());
                Gizmos.DrawCube(uv3cen, uv3size);
                count++;
            }

            foreach (Ray collisionCheck in lastRayCollisionChecks)
            {
                Gizmos.color = new Color(1.0f, 1.0f - ((float)count / numCollisionsToSave), 1.0f);
                Gizmos.DrawRay(collisionCheck.origin, collisionCheck.direction);
                count++;
            }

            Gizmos.color = Color.white;
        }

        // #### PRIVATE METHODS ####

        /// <summary>
        /// Used for visualising collision checks with DrawCollisionChecks.
        /// Automatically removed from builds so that collision checks aren't slowed down.
        /// </summary>
        /// <param name="checkBounds">bounds that were passed in to check for collisions.</param>
        void AddCollisionCheck(LRect checkBounds)
        {
            lastBoundsCollisionChecks.Enqueue(checkBounds);
            if (lastBoundsCollisionChecks.Count > numCollisionsToSave)
            {
                lastBoundsCollisionChecks.Dequeue();
            }
        }

        /// <summary>
        /// Used for visualising collision checks with DrawCollisionChecks.
        /// Automatically removed from builds so that collision checks aren't slowed down.
        /// </summary>
        /// <param name="checkRay">ray that was passed in to check for collisions.</param>
        void AddCollisionCheck(Ray checkRay)
        {
            lastRayCollisionChecks.Enqueue(checkRay);
            if (lastRayCollisionChecks.Count > numCollisionsToSave)
            {
                lastRayCollisionChecks.Dequeue();
            }
        }
    }
}
#endif
