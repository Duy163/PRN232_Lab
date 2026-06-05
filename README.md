# COVID19 Project

Đây là hướng dẫn chạy project backend và frontend của bài tập.

## Cấu trúc thư mục

- `backend/COVID19/COVID19` là backend ASP.NET Core Web API
- `frontend` là frontend React + Vite

## Yêu cầu trước khi chạy

- Cài đặt **.NET SDK 8.0**
- Cài đặt **Node.js** và **npm**
- Có **SQL Server** đang chạy
- Cập nhật chuỗi kết nối database trong file:
  - `backend/COVID19/COVID19/appsettings.json`
  - hoặc `backend/COVID19/COVID19/appsettings.Development.json`

## Chạy backend

Mở terminal tại thư mục `backend/COVID19` rồi chạy:

```bash
cd backend/COVID19
```

### 1) Cài package nếu cần

Thông thường chỉ cần restore khi build:

```bash
dotnet restore
```

### 2) Chạy migration

Nếu dự án đã có migration sẵn, chạy:

```bash
dotnet ef database update --project COVID19/COVID19.csproj --startup-project COVID19/COVID19.csproj
```

Nếu máy chưa nhận lệnh `dotnet ef`, cài tool EF Core:

```bash
dotnet tool install --global dotnet-ef
```

Sau đó chạy lại lệnh migration ở trên.

### 3) Chạy backend

```bash
dotnet run --project COVID19/COVID19.csproj
```

Backend thường sẽ chạy ở địa chỉ hiển thị trong terminal, ví dụ:
- `https://localhost:xxxx`
- `http://localhost:xxxx`

## Chạy frontend

Mở terminal tại thư mục `frontend` rồi chạy:

### 1) Cài thư viện

```bash
npm install
```

### 2) Chạy frontend

```bash
npm run dev
```

Nếu muốn build để kiểm tra bản production:

```bash
npm run build
```

## Hướng dẫn nộp bài

Trước khi nén file nộp:

- Có thể xóa `frontend/node_modules`
- Có thể xóa `backend/COVID19/COVID19/bin`
- Có thể xóa `backend/COVID19/COVID19/obj`
- Giữ lại source code, file `.csproj`, `package.json`, và các file cấu hình cần thiết

## Ghi chú

Nếu backend không kết nối được database, hãy kiểm tra lại:
- SQL Server đã bật chưa
- tên server trong connection string đúng chưa
- database đã được tạo bằng migration chưa

Nếu frontend không gọi được API, hãy kiểm tra lại URL API trong code frontend.
