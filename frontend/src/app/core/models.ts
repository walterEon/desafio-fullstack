export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface Product {
  id: number;
  categoriaId: number;
  nombre: string;
  descripcion: string;
  precio: number;
  stock: number;
  imagenUrl: string | null;
  categoriaNombre: string;
}

export interface ProductUpdate {
  nombre: string;
  descripcion: string;
  precio: number;
  stock: number;
  categoriaId: number;
}

export interface Category {
  id: number;
  nombre: string;
}

export interface ApiError {
  message: string;
}
