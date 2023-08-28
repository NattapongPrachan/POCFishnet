using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UniRx;
using System;

public class GameSetting : MonoSingleton<GameSetting>
{
    [SerializeField]UniversalRenderPipelineAsset _urpPipelineAsset;
    [SerializeField] Texture2D _cursorTexture;

    public ReactiveProperty<float> _cameraSensitivity = new ReactiveProperty<float>(0.5f);
    public ReactiveProperty<float> _cameraXAxisMultiply = new ReactiveProperty<float>(200);
    public ReactiveProperty<float> _cameraYAxisMultiply = new ReactiveProperty<float>(4);
    public ReactiveProperty<float> _renderScale = new ReactiveProperty<float>(1);
    public ReactiveProperty<float> _renderMultiply = new ReactiveProperty<float>(1000);
    public ReactiveProperty<bool> _isAutoRun = new ReactiveProperty<bool>(false);
    public event Action<bool> onUseCursor;

    public override void Init()
    {
        base.Init();
        Cursor.SetCursor(_cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        SetCursorToUse(true);
        //PersistentDataCenter.OnLocalFileLoaded += AddListenerPersistentData;
        
    }

    private void AddListenerPersistentData()
    {
        // PersistentDataCenter.OnLocalFileLoaded -= AddListenerPersistentData;
        // PersistentDataCenter.Instance.DisplaySettingData.ObserveEveryValueChanged(d => d.Data.graphicQuality).Subscribe(_ =>{
        //     this.SetQualityLevel(_,false);
        // }).AddTo(this);
        // PersistentDataCenter.Instance.DisplaySettingData.ObserveEveryValueChanged(d =>d.Data.fps).Subscribe(_ =>{
        //     Application.targetFrameRate = _;
        //     QualitySettings.vSyncCount = (_ == 60) ? 1 : 2;
        // }).AddTo(this);
        // PersistentDataCenter.Instance.DisplaySettingData.ObserveEveryValueChanged(d => d.Data.renderScale).Subscribe(_ =>{
        //     _renderScale.Value = _;
        // }).AddTo(this);
        // PersistentDataCenter.Instance.AudioSettingData.ObserveEveryValueChanged(d => d.Data.master).Subscribe(_ =>{
        //     SoundManager.Instance.GlobalVolume = _;
        // }).AddTo(this);
        // PersistentDataCenter.Instance.AudioSettingData.ObserveEveryValueChanged(d => d.Data.bgm).Subscribe(_ =>{
        //     SoundManager.Instance.GlobalMusicVolume = _;
        // }).AddTo(this);
        // PersistentDataCenter.Instance.AudioSettingData.ObserveEveryValueChanged(d => d.Data.soundFX).Subscribe(_ =>{
        //     SoundManager.Instance.GlobalSoundsVolume = _;
        // }).AddTo(this);
        // PersistentDataCenter.Instance.AudioSettingData.ObserveEveryValueChanged(d => d.Data.soundUI).Subscribe(_ =>{
        //     SoundManager.Instance.GlobalUISoundsVolume = _;
        // }).AddTo(this);
        // PersistentDataCenter.Instance.ControlSettingData.ObserveEveryValueChanged(d => d.Data.autoRun).Subscribe(_ =>{
        //     _isAutoRun.Value = _;
        // }).AddTo(this);
        // PersistentDataCenter.Instance.ControlSettingData.ObserveEveryValueChanged(d => d.Data.cameraSensitivity).Subscribe(_ =>{
        //    _cameraSensitivity.Value = _;
        // }).AddTo(this);
    }

    public void SetQualityLevel(int index,bool applyExpensiveChange = false)
    {
        QualitySettings.SetQualityLevel(index,applyExpensiveChange);
        
    }

    public void SetCursorToUse(bool isUsed)
    {
        onUseCursor?.Invoke(isUsed);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = isUsed;
    }

}
