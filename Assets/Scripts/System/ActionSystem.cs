using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;

    public bool isPerforming {get; private set;} = false;

    private static Dictionary<Type, List<Action<GameAction>>> preSubscribe = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubscribe = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    public void Perform (GameAction action, System.Action OnPerfomFinished = null)
    {
       if (isPerforming) return;
       isPerforming = true;
       StartCoroutine(Flow(action, () =>
       {
           isPerforming = false;
           OnPerfomFinished?.Invoke();
       }));
    }
    public void AddReaction (GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubscribe);
        yield return PerformReaction();

        reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReaction();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubscribe);
        yield return PerformReaction();

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformReaction ()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);    
        }
        
    }
    private IEnumerator PerformPerformer (GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);    
        }
    }
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subscribers)
    {
        Type type = action.GetType();
        if (subscribers.ContainsKey(type))
        {
            foreach (var subscriber in subscribers[type])
            {
                subscriber(action);
            }
        }
    }
    public static void AttachPerformer<T> (Func<T, IEnumerator> performer) where  T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else performers.Add(type, wrappedPerformer);
    }
    public static void DetachPerformer<T> () where  T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }
    public static void SubscribePerformer<T> (Action<T> reaction, ReactionTiming timing) where  T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubscribe : postSubscribe;
        void wrappedReaction (GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        } else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);    
        }
    }
    public static void UnSubscribePerformer<T> (Action<T> reaction, ReactionTiming timing) where  T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubscribe : postSubscribe;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction (GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }
}
