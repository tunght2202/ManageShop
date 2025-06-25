# ManageShop - Ứng dụng quản lý siêu thị với WPF  

![WPF](https://img.shields.io/badge/.NET-WPF-blue)  
![C#](https://img.shields.io/badge/Language-C%23-green)  
![MVVM](https://img.shields.io/badge/Pattern-MVVM-purple)  


**ManageShop** là ứng dụng quản lý siêu thị được phát triển bằng **WPF (.NET)**, hỗ trợ 2 phân quyền chính:  

1. **Thu ngân (Cashier)**  
*Tài khoản thu : admin / Mật khẩu: 1234*
   - Xử lý bán hàng, tạo đơn hàng.  
   - Tính tiền.  

2. **Quản lý (Admin)**  
*Tài khoản admin: sale / Mật khẩu: 1234*
   - **(ManageProducts)**  
   - **(ManageCustomers)** 
   - **(ManageOrders)**

## Công nghệ sử dụng  
- **Ngôn ngữ**: C#  
- **Giao diện**: WPF (Windows Presentation Foundation)  
- **Cơ sở dữ liệu**: SQL Server / MySQL  

## Hình ảnh demo

### 1. Giao diện đăng ký 
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/RegisterEmployee.png" width="300">
  <p><i>Ảnh 1: Register</i></p>
</div>
*Chức năng: Tạo tài khoản cho nhân viên thu ngân mới.*


### 2. Đăng nhập 
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/Login.png" width="600">
  <p><i>Ảnh 2: Login</i></p>
</div>
*Chức năng: Đăng nhập với vai trò thu ngân hoặc vai trò quản trị viên*

#### 2.1. Đăng nhập với vai trò quản trị viên
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/AdminLogin.png" width="600">
  <p><i>Ảnh 2.1: Login with admin role</i></p>
</div>
*Chức năng: Đăng nhập với vai trò trò quản trị viên*

#### 2.2. Đăng nhập với vai trò thu ngân
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/EmployeeLogin.png" width="600">
  <p><i>Ảnh 2.2: Login with employee role</i></p>
</div>
*Chức năng: Đăng nhập với vai trò trò nhân viên thu ngân*


### 3. Bảng lựa chọn. *(admin role)*
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/Dashboard.png" width="600">
  <p><i>Ảnh 3: Dashboard </i></p>
</div>
*Chức năng: Cho phép người dùng lựa chọn các chức năng như: quản lý sản phẩm, quản lý khách hàng và quản lý đơn hàng*


### 4. Quản lý sản phẩm. *(admin role)*
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/ProductManagement.png" width="600">
  <p><i>Ảnh 4: ManageProducts </i></p>
</div>
*Chức năng: Quản trị viên quản lý sản phẩm trong siêu thị*


### 5. Quản lý khách hàng. *(admin role)*
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/CustomerManage.png" width="600">
  <p><i>Ảnh 5: ManageCustomers </i></p>
</div>
*Chức năng: Quản trị viên quản lý khách hàng trong siêu thị*


### 6. Quản lý đơn hàng. *(admin role)*
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/OrderManagement.png" width="600">
  <p><i>Ảnh 6: ManageOrders </i></p>
</div>
*Chức năng: Quản trị viên quản lý đơn hàng trong siêu thị*


### 7. Màn hình bán hàng. *(Cashier role)*
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/SaleSupermartket.png" width="600">
  <p><i>Ảnh 7: Seller </i></p>
</div>
*Chức năng: Nhân viên thu ngân thêm các sản phẩm vào đơn hàng*


### 8. Màn hình đơn hàng. *(Cashier role)*
<div align="center">
  <img src="https://github.com/tunght2202/ManageShop/blob/master/ManagerShopImage/Order.png" width="600">
  <p><i>Ảnh 7: Order </i></p>
</div>
*Chức năng: Nhân viên thu ngân thêm thông tin khách hàng và tạo đơn hàng*
