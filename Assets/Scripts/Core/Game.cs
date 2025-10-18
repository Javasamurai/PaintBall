using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Game : ILifeCycle
    {
        private AudioService _audioService;
        
        public static Game Instance { get; private set; }
        
        private static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        public Game()
        {
            if (Instance != null)
            {
                throw new System.Exception("Game instance already exists.");
            }
            Instance = this;
        }

        public void Initialize()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
            
            InitializeServices();
            CreateWorld();
        }

        private void CreateWorld()
        {
            
        }

        public void Update()
        {
            foreach (var service in _services.Values)
            {
                service.Update();
            }
        }


        private void InitializeServices()
        {
            _audioService = new AudioService();
            _audioService.Initialize();
            _services[typeof(AudioService)] = _audioService;

            foreach (var service in _services.Values)
            {
                service.Initialize();
            }
        }
        
        public static T GetService<T>() where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T) service;
            }
            throw new Exception($"Service of type {typeof(T)} is not registered.");
        }

        public void Shutdown()
        {
            Instance = null;
            
            foreach (var service in _services.Values)
            {
                service.Shutdown();
            }
        }
    }
}