import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit, inject, input, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CatalogService } from '../../core/catalog.service';
import { ApiError, Category, Product } from '../../core/models';

@Component({
  selector: 'app-product-edit',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './product-edit.html',
  styleUrl: './product-edit.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductEdit implements OnInit {
  private readonly catalog = inject(CatalogService);

  /** Parámetro :id de la ruta. */
  readonly id = input.required<string>();

  protected readonly form = inject(NonNullableFormBuilder).group({
    nombre: ['', Validators.required],
    descripcion: [''],
    precio: [0, [Validators.required, Validators.min(0)]],
    stock: [0, [Validators.required, Validators.min(0)]],
    categoriaId: [0, [Validators.required, Validators.min(1)]],
  });

  protected readonly product = signal<Product | null>(null);
  protected readonly categories = signal<Category[]>([]);
  protected readonly loading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected readonly success = signal<string | null>(null);
  protected readonly saveError = signal<string | null>(null);

  ngOnInit(): void {
    forkJoin({
      product: this.catalog.getProduct(Number(this.id())),
      categories: this.catalog.getCategories(),
    }).subscribe({
      next: ({ product, categories }) => {
        this.categories.set(categories);
        this.setProduct(product);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.loadError.set(
          err instanceof HttpErrorResponse && err.status === 404
            ? 'El producto no existe.'
            : 'No se pudo cargar el producto.',
        );
        this.loading.set(false);
      },
    });
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.success.set(null);
    this.saveError.set(null);
    this.saving.set(true);

    this.catalog.updateProduct(Number(this.id()), this.form.getRawValue()).subscribe({
      next: (updated) => {
        this.setProduct(updated);
        this.saving.set(false);
        this.success.set('Producto actualizado correctamente.');
      },
      error: (err: unknown) => {
        this.saving.set(false);
        this.saveError.set(this.errorMessage(err));
      },
    });
  }

  private setProduct(product: Product): void {
    this.product.set(product);
    this.form.reset({
      nombre: product.nombre,
      descripcion: product.descripcion ?? '',
      precio: product.precio,
      stock: product.stock,
      categoriaId: product.categoriaId,
    });
  }

  private errorMessage(err: unknown): string {
    if (err instanceof HttpErrorResponse && err.status === 400) {
      const body = err.error as Partial<ApiError> | null;
      if (body?.message) {
        return body.message;
      }
    }
    return 'No se pudo guardar el producto.';
  }
}
