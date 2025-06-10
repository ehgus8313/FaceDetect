# �絹�� �� �ռ���

## ����

�� ������Ʈ�� C#/WPF ����� ���� ó�� ���ø����̼�����, **��ķ �Ǵ� �̹��� ���� �� ���� �ν�**�ϰ�, **�� ������ ������ �� �Ǵ� �̹���(PNG)**�� �ռ��Ͽ� �絹��ó�� ǥ�����ִ� ���α׷��Դϴ�.

- �� ����: `dlib` ���
- �̹��� ó��: `OpenCV`
- UI ����: `WPF + MVVM ����`
- �Է�: ��ķ �ǽð� ���� / ���� �̹���/���� ����
- ���: �󱼿� �絹�� �ڰ� �ռ��� ��� ���� or �̹���

---

## ? �ֿ� ���

- ? **��ķ �ǽð� �� �ν�**
- ? **�̹��� | ���� ���Ͽ��� �� ����**
- ? **�� ��ġ�� ���� �� �Ǵ� PNG �ռ� (�絹�� ��)**
- ? **UI�󿡼� ���� ����� ��ķ ���� ��ȯ**
- ? **������ ����� ��� (���� ����)**

---

## ? ���α׎c �̹���

![program_1](./Assets/program_1.png)
![program_2](./Assets/program_2.png)

---

## ? ���� �̹���

| �Է� �̹��� | ��� �̹��� |
|-------------|-------------|
| ![input example](./Assets/example_input_1.jpg) | ![output example](./Assets/example_output_1.png) |
| ![input example](./Assets/example_input_2.jpg) | ![output example](./Assets/example_output_2.png) |

> �̹����� ǥ�õ��� �ʴ� ���, `Assets` ���� ����
`example_input_1.jpg`, `example_input_2.jpg`, `example_output_1_.png`, `example_output_2_.png` ������ Ȯ���ϼ���.

---

## ? ��� ��� ����

| ���� | ��� ��� |
|------|-----------|
| ��� | C# |
| UI �����ӿ�ũ | WPF (MVVM ���� ����) |
| �̹���/���� ó�� | OpenCvSharp |
| �� �ν� | dlib-dotnet |
| ���� ��Ʈ���� | OpenCV VideoCapture |
| ��ķ ����Ʈ | DirectShowLib |
| ����� ��� | MediaPlayer (����) |

---

## ? ������Ʈ ����

```bash
��
������ Models/
�� ������ FileData.cs # �̹���/���� ���� ���� ��
��
������ ViewModels/
�� ������ MainViewModel.cs # MVVM ������ �߽� ViewModel
�� ������ ViewModelBase.cs # ViewModel Base
��
������ Views/
�� ������ MainWindow.xaml # �⺻ UI ȭ��
�� ������ SelectWebcamDialog.xaml # ��ķ ���� �˾�
��
������ Utils/
�� ������ ImageProcessor.cs # �� �ν� + �絹�� �� �ռ� ó��
�� ������ CameraHelper.cs # DirectShowLib ��� ��ķ ����Ʈ ����
��
������ Assets/
�� ������ nose.png # �絹�� �� �̹��� (�ɼ�)
��
������ README.md # �� ����
``` 

## ��? ���� ���

1. **�ʼ� ���� Ȯ��**
   - `shape_predictor_68_face_landmarks.dat`: �� landmark �� (dlib ���� ��)
     - [dlib-models](https://github.com/davisking/dlib-models/blob/master/shape_predictor_68_face_landmarks.dat.bz2)
     - `������/dlib-model/` ���丮�� `shape_predictor_68_face_landmarks.dat` ���� ��ġ�ؾ� ��
   - `Assets/nose.png`: �絹�� �� PNG (optional)
     - �ش� ������ ���� ��� **�ڵ����� ���� ������ ��ü��**

2. **������Ʈ ����**
   - Visual Studio���� `.sln` ���� ����
   - `AnyCPU`, `Debug` �Ǵ� `Release` ���� �� ����

3. **���α׷� ����**
   - ��� �ǿ��� `�̹���` �Ǵ� `����` ����
   - `���� �ҷ�����`: mp4 ���� ���� �� �� ���� + �絹�� �� �ռ�
   - `��ķ ����`: ����� ī�޶� ��ġ ���� �� �ǽð� �ռ�

4. **����**
   - ���� �� `����` ��ư Ŭ������ ����/��ķ ���� ����

---

## ?? ���ʽ� ��� ����

- `nose.png`�� ������ ���: �� ������ �ش� �̹����� �ռ��մϴ�. �ǻ�ü�� ��ġ�� ���� �� ������ �̹��� ũ�Ⱑ ����˴ϴ�. (��� ���� PNG ����)
- `nose.png`�� ���� ���: �ڵ����� **���� ��(circle)**�� �� ��ġ�� �׷����ϴ�
- `MediaPlayer`�� ���� **������ ����� ���** ����: mp4 ���� ��� �� �Ҹ� ��� ���θ� ����ڿ��� ���� �˾� ����
- ��ķ ������ �ý��� ����� ��ġ ����Ʈ���� ����ڿ��� ���� UI ���� (WMI ���)

---

## ??��? ������ ����

- �̸�: �ǵ���
- �̸���: ehgsu8313@naver.com
- �ֿ� ���̺귯�� ��ó:
  - [OpenCvSharp](https://github.com/shimat/opencvsharp)
  - [dlib-dotnet](https://github.com/takuya-takeuchi/DlibDotNet)
  - [DirectShowLib](https://github.com/larrybeall/DirectShowLib)