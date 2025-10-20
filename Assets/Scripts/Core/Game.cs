using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Core
{
    public class Game : ILifeCycle
    {
        private AudioService _audioService;
        private NetworkService _networkService;
        private SceneService _sceneService;
        private PlayerServices _playerServices;
        private GameConfig _gameConfig;
        
        public static bool IsReady { get; private set; }
        public static Game Instance { get; private set; }
        
        private static readonly Dictionary<Type, IService> _services = new ();

        public Game(GameConfig gameConfig)
        {
            if (Instance != null)
            {
                throw new Exception("Game instance already exists.");
            }
            Instance = this;
            this._gameConfig = gameConfig;
        }

        public void Initialize()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
            InitializeServices();
            IsReady = true;
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
            _networkService = new NetworkService();
            _sceneService = new SceneService();
            _playerServices = new PlayerServices();
            
            _services[typeof(NetworkService)] = _networkService;
            _services[typeof(AudioService)] = _audioService;
            _services[typeof(SceneService)] = _sceneService;
            _services[typeof(PlayerServices)] = _playerServices;
            
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