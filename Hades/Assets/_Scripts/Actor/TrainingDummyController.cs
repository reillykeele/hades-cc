using UnityEngine;

namespace Actor
{
    public class TrainingDummyController : MonoBehaviour, IDamagable
    {
        private Animator _anim;

        private bool _invuln;
        private float _timeSinceLastDamage = 0f;
        private float _recoveryTime = 0.5f;

        void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (_invuln)
            {
                if (_timeSinceLastDamage < _recoveryTime)
                {
                    _timeSinceLastDamage += Time.deltaTime;
                }
                else
                {
                    _invuln = false;
                }
            }
        }

        public void Damage(float damage)
        {
            if (_invuln == false)
            {
                _anim.SetTrigger("hit");

                _invuln = true;
                _timeSinceLastDamage = 0.0f;
            }
        }
    }
}
