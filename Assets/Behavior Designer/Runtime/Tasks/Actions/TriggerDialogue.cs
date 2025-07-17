using UnityEngine;
using Yarn.Unity;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TriggerDialogue : Action
    {
        private DialogueRunner dialogueRunner;

        public override void OnStart()
        {
            dialogueRunner = GameObject.FindAnyObjectByType<DialogueRunner>();
            dialogueRunner.StartDialogue("GetAway");
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            dialogueRunner = null;
        }
    }
}