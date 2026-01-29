# GlobalGameJam

## 📦Cấu trúc dự án

```
├── Materials/              # Chứa các material của game
├── Prefabs/                # Chứa các prefab của game
├── Scenes/                 # Chứa các scene của game
│  ├── MainScene.unity      # Scene chính khi vừa mở game
├── Scripts/                # Chứa các script của game
│  ├── Singleton.cs         # Script quản lý singleton
├── Settings/               # Chứa các setting chung của project
├── Render Feattures/       # Share vẽ outline (không đụng vào)
```

## Cách sử dụng

### Singleton.cs

- Đây là một lớp cơ sở để tạo các singleton trong Unity.
- Singleton là một mẫu thiết kế đảm bảo rằng một lớp chỉ có một thể hiện duy nhất và cung cấp một điểm truy cập toàn cục
  đến thể hiện đó.
- Để sử dụng lớp Singleton, bạn chỉ cần kế thừa từ nó trong lớp của bạn. Ví dụ:
- ```csharp
    public class GameManager : Singleton<GameManager>
    {
        // Các thuộc tính và phương thức của GameManager
    }
    ```

## Assets credits

- Các assets sử dụng trong game đều được lấy từ các trang web cung cấp tài nguyên miễn phí như:
    - [Free Cyberpunk Backgrounds Pixel Art](https://free-game-assets.itch.io/free-scrolling-city-backgrounds-pixel-art)