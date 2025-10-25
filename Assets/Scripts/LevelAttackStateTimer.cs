using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelAttackStateTimer
{
    public struct TimerEvent {
        public float triggerTime;
        public Enemy.EnemyState state;
        public bool triggered;
    }

    private readonly List<TimerEvent> events = new();
    private readonly Enemy[] enemies;
    private readonly Action<Enemy.EnemyState> setLastState;

    private float elapsed = 0f;
    private int evIdx = 0;
    private TimerEvent currEv;
    public bool active = false;

    public LevelAttackStateTimer(List<TimerEvent> events, Enemy[] enemies, Action<Enemy.EnemyState> setLastState) {
        this.events = events;
        this.enemies = enemies;
        this.setLastState = setLastState;
        Reset();
    }

    public void Start() { active = true; }
    public void Stop() { active = false; }
    public void Reset() {
        elapsed = 0f;
        evIdx = 0;
        for (int i = 0; i < events.Count; i++) {
            TimerEvent evt = events[i];
            evt.triggered = false;
            events[i] = evt;
        }
        setLastState.Invoke(Enemy.EnemyState.Scatter);
    }

    public void Update() {
        if (!active || evIdx >= events.Count) { return; }
        elapsed += Time.deltaTime;
        currEv = events[evIdx];
        if (elapsed >= currEv.triggerTime && !currEv.triggered) {
            setLastState.Invoke(currEv.state);
            foreach (Enemy e in enemies) {
                e.behaviour.SwapAttackState(currEv.state);
            }
            currEv.triggered = true;
            events[evIdx++] = currEv;
        }
    }
}
