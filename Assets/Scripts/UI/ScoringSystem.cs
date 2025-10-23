using System;
using Core;
using Unity.Collections;
using Unity.Entities;

namespace UI
{

    public class ScoreUI : UIView
    {
        private void Start()
        {
            
        }
    }
    public struct ScoreComponent : IComponentData
    {
        public int CurrentScore;
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class ScoringSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            EntityQueryBuilder queryBuilder = new EntityQueryBuilder(Allocator.Temp);
            queryBuilder.WithAll<ScoreComponent>();
            
            EntityQuery scoreQuery = GetEntityQuery(queryBuilder);
            RequireForUpdate(scoreQuery);
        }
        protected override void OnUpdate()
        {
            foreach (var (score, entity) in SystemAPI.Query<RefRW<ScoreComponent>>().WithEntityAccess())
            {
                
                // TODO: Update the score UI with the current score
            }
        }
    }
}