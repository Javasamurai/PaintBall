using Core;
using Systems.Gameplay;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : UIView
    {
        [SerializeField]
        private Image healthBarImage;
        
        private void Awake()
        {
            foreach (var world in World.All)
            {
                if (world.IsClient() && !world.IsThinClient())
                {
                    var healthUISystem = world.GetOrCreateSystemManaged<HealthUISystem>();
                    healthUISystem.healthBar = this;
                    var simulationSystemGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
                    simulationSystemGroup.AddSystemToUpdateList(healthUISystem);
                }
            }
        }
        public void SetHealth(float health)
        {
            healthBarImage.fillAmount = health / 100f;

            switch (health)
            {
                case <= 30:
                    healthBarImage.color = Color.red;
                    break;
                case < 50:
                    healthBarImage.color = Color.yellow;
                    break;
                default:
                    healthBarImage.color = Color.green;
                    break;
            }
        }

        public void SetMaxHealth(float maxHealth)
        {
            healthBarImage.fillAmount = maxHealth / 100f;
        }
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [DisableAutoCreation]
    public partial class HealthUISystem : SystemBase
    {
        public HealthBar healthBar;
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<HealthComponent>(), ComponentType.ReadOnly<GhostOwnerIsLocal>()));
        }
        protected override void OnUpdate()
        {
            CompleteDependency();
            foreach (var (healthComponent, entity) in SystemAPI.Query<RefRO<HealthComponent>>().WithAny<GhostOwnerIsLocal>().WithEntityAccess())
            {
                healthBar.SetHealth(healthComponent.ValueRO.CurrentHealth);
            }
        }
    }
}