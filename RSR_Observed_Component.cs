using EFT;
using EFT.Animations;
using EFT.UI;
using Fika.Core.Coop.Players;
using UnityEngine;

namespace StanceReplication
{
    public class RSR_Observed_Component : MonoBehaviour
    {

        float _allowStanceTimer = 0f;
        float _cancelStanceTimer = 0f;
        bool _canUpdateStance = false;

        float sprintAnim = 1f;
        bool _isPatrol = false;
        bool doPatrol = false;
        Vector3 _targetPosition = Vector3.zero;
        Vector3 _packetPosition = Vector3.zero;
        Quaternion _packetRotation = Quaternion.identity;
        Quaternion _targetRotation = Quaternion.identity;
        ObservedCoopPlayer _observedCoopPlayer;

        private void Start()
        {
            _observedCoopPlayer = GetComponent<ObservedCoopPlayer>();
            _observedCoopPlayer.OnPlayerDead += DeleteThis;
            Plugin.ObservedComponents.Add(_observedCoopPlayer.NetId, this);
        }

        private void DeleteThis(EFT.Player player, EFT.IPlayer lastAggressor, DamageInfo damageInfo, EBodyPart part)
        {
            _observedCoopPlayer.OnPlayerDead -= DeleteThis;
            Destroy(this);
        }

        private Quaternion ScaleRotation(Quaternion q, float factor)
        {
            q.ToAngleAxis(out float angle, out Vector3 axis);
            float scaledAngle = angle * factor;
            return Quaternion.AngleAxis(scaledAngle, axis);
        }

        private void Update()
        {

            /* 
             
             Issue Raised by Traveller
            
             [Exception] : NullReferenceException
            UnityEngine.Animator.SetBool (System.Int32 id, System.Boolean value) (at <8200f53961d4421c8c1d6290273b47b5>:0)
            GClass1247.SetBool (System.Int32 valueNameHash, System.Boolean value) (at <a5b6eaefee6842aba73e4a77ea14a564>:0)
            WeaponAnimationSpeedControllerClass.SetPatrol (IAnimator animator, System.Boolean patrol) (at <a5b6eaefee6842aba73e4a77ea14a564>:0)
            ObjectInHandsAnimator.SetPatrol (System.Boolean b) (at <a5b6eaefee6842aba73e4a77ea14a564>:0)
            EFT.MovementContext.SetPatrol (System.Boolean b) (at <a5b6eaefee6842aba73e4a77ea14a564>:0)
            StanceReplication.RSR_Observed_Component.Update () (at <170bb6ad5e534f6fafc4204d765c681a>:0)
            UnityEngine.DebugLogHandler:LogException(Exception, Object)
            Class362:UnityEngine.ILogHandler.LogException(Exception, Object)
            UnityEngine.Debug:CallOverridenDebugHandler(Exception, Object)
            */

            /*
             * Notes:
             * From Fontaine on Fika Discord
                if it's not being done already, check to make sure Player.FirearmController is not null before setting patrol stance
            
            
             * Fontaine: OP
                 — Today at 5:18 AM
                Solarint told me he had this sort of error reported before since he added the bot patrol stance feature to SAIN. I've never seen it with Realism though.
             
             * This stack trace provided by Traveler did occur in Primal-13's playgroup as well
             * But only when the match was loading. Errors did stop eventually and it was noted that the individual was experiencing alot of packet loss
             */


            if (_observedCoopPlayer == null)
            {
                return;
            }

            ProceduralWeaponAnimation pwa = _observedCoopPlayer.ProceduralWeaponAnimation;


            if (_observedCoopPlayer.IsSprintEnabled || !_observedCoopPlayer.ProceduralWeaponAnimation.OverlappingAllowsBlindfire)
            {
                _cancelStanceTimer += Time.deltaTime;
                if (_cancelStanceTimer >= Plugin.CancelTimer.Value)
                {
                    _allowStanceTimer = 0f;
                    _canUpdateStance = false;
                }
            }
            else if (!_canUpdateStance)
            {
                _cancelStanceTimer = 0f;
                _allowStanceTimer += Time.deltaTime;
                if (_allowStanceTimer >= Plugin.ResetTimer.Value)
                {
                    _canUpdateStance = true;
                }
            }

            if (_canUpdateStance)
            {
                doPatrol = _isPatrol;

                _targetPosition = Vector3.Lerp(_targetPosition, _packetPosition, 0.5f);
                _targetRotation = Quaternion.Slerp(_targetRotation, _packetRotation, 0.5f);

                /*Quaternion factoredTargetRoation = _targetRotation;
                if (_packetRotation.x != 0f || _packetRotation.y != 0f || _packetRotation.z != 0f) 
                {
                    factoredTargetRoation = ScaleRotation(_targetRotation, Plugin.Test2.Value);
                }
*/
                pwa.HandsContainer.HandsPosition.Zero = pwa.PositionZeroSum + _targetPosition * pwa.Single_3 * 1.5f;
                pwa.HandsContainer.WeaponRootAnim.rotation = Quaternion.Slerp(pwa.HandsContainer.WeaponRootAnim.rotation, _targetRotation, 1f);
            }
            else
            {
                doPatrol = false;
                _targetPosition = Vector3.Lerp(_targetPosition, Vector3.zero, 0.5f);
                _targetRotation = Quaternion.Slerp(_targetRotation, Quaternion.identity, 0.5f);
            }

            _observedCoopPlayer.MovementContext.SetPatrol(doPatrol);
            _observedCoopPlayer.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, sprintAnim);
        }

        public void SetAnimValues(Vector3 weapPos, Quaternion rot, bool patrol, float anim)
        {
            _packetPosition = weapPos;
            _packetRotation = rot;
            _isPatrol = patrol;
            sprintAnim = anim;

        }
    }
}
