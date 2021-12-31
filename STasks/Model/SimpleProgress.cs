using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STasks.Model.Bases;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;

namespace STasks.Model
{

    public class SimpleProgress : ProgressObject
    {
        /// <summary>
        /// other denotes changes that happen on construction and loading time, 
        /// </summary>
        public enum SimpleProgressValueChangeSource { UserAction, UserActionThroughAncestor, Other=0  }
        public event EventHandler<SimpleProgressValueChangeSource> ValueChanged;
        public SimpleProgress(STBuildingBlock ttachedBB, int weight = 1, bool value = false)
        {
            Weight = weight; Value = value;
            STBBObject = ttachedBB;
            Weight = weight; Value = value;
        }


        private void OnValueChanged(SimpleProgressValueChangeSource source)
        {
            ValueChanged?.Invoke(this, source);
        }

        //SIMPLE prog can be set externally by user
        // woplex progress cannot be set by user

        //Simple prog always have determined percent value
        // complex prog's percent value can be indetermined in case the underlying STOBjetcs graph doesn contain building blocks, only containers



        private bool _Value;
        public bool Value
        {
            set
            {
                bool old = _Value;
                _Value = value;
                if (old != value) 
                    notif(nameof(IsComplete));
            }
            get { return _Value; }
        }


        public int Weight { get; private set; }

        public STBuildingBlock STBBObject;
        public override bool IsDetermined { get { return true; } }

        public override float Ratio { get { return Value ? 1 : 0; } }
        public override bool IsComplex { get { return false; } }

        public override int CompletedCount { get { throw new Exception("SimpleProgress Doesnt support CompletedCount"); } }

        public override int TotalCount { get { throw new Exception("SimpleProgress Doesnt support TotalCount"); } }

        public override bool IsComplete { get { return Value; } }

        internal override void SetCompletion()
        {
            Value = true;
        }
        internal override void ResetCompletion()
        {
            Value = false;
        }

        internal override void HandleNotificaton<T>(InternalProgressTreeNotificationArgs<T> e)
        {
            base.HandleNotificaton<T>(e);
            if (e.Type == ProgressNotificationType.AncestorComplexProgGotCompletionRequire)
            {
                bool value = (bool)(object)e.Data;
                if (value) SetCompletion();
                else ResetCompletion();
            }
        }

        internal override ProgressObject ParentProgressObject { get { return STBBObject?.Parent?.Progress; } }

        internal override IEnumerable<ProgressObject> ChildProgressObjects { get { return null; } }


        public void ApplyProgressValue(bool newVal, int newWeight)// user sourced action, should propagate up
        {
            Debug.WriteLine($"Applying prog: val: {newVal} w:{newWeight}");
            ApplyProgressArgs old_ = new ApplyProgressArgs() { Value = Value, Weight = Weight };

            ApplyProgressArgs new_ = new ApplyProgressArgs() { Value = newVal, Weight = newWeight };
            if (old_.Weight != new_.Weight || old_.Value != new_.Value)//if something has changeed happened
            {

                Value = newVal;
                Weight = newWeight;
                PropagateNotificationUp(new InternalProgressTreeNotificationArgs<NewOld<ApplyProgressArgs>>(this, ProgressNotificationType.SimpleProgressGotApllied, new NewOld<ApplyProgressArgs>(new_, old_)));

            }
        }
        public override void RequireCompletion(bool newVal)//for consistency witth complex progress
        {

            ApplyValue(newVal);



        }

        /// <summary>
        /// shorthand for applyProgress when only one prop need to be updated
        /// </summary>
        public void ApplyValue(bool v)
        {
            ApplyProgressValue(v, Weight);
        }

        public void ApplyWeight(int w)
        {
            ApplyProgressValue(Value, w);
        }

        public static AdditiveProgress operator +(SimpleProgress sp1, SimpleProgress sp2)
        {
            Func<bool, int> asInt = (bool b) => { return b ? 1 : 0; };
            return new AdditiveProgress()
            {
                CompletedCount = (asInt(sp1.IsComplete) + asInt(sp2.IsComplete)),
                CompletedWeight = (asInt(sp1.IsComplete) * sp1.Weight + asInt(sp2.IsComplete) * sp2.Weight),
                TotalCount = 2,
                TotalWeight = sp1.Weight + sp2.Weight
            };
        }
    }

}
