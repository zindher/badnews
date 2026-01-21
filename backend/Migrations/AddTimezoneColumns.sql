-- Script SQL para agregar soporte de zonas horarias a la tabla Orders
-- Ejecutar este script si prefieres actualizar la BD manualmente en lugar de usar migrations

-- Agregar nuevas columnas a la tabla Orders
ALTER TABLE [dbo].[Orders]
ADD 
    [PreferredCallTime] NVARCHAR(5) NULL,
    [RecipientTimezone] NVARCHAR(20) NULL,
    [RecipientState] NVARCHAR(100) NULL;

-- Crear índices para mejorar queries por zona horaria y horario
CREATE NONCLUSTERED INDEX [IX_Orders_RecipientTimezone]
ON [dbo].[Orders] ([RecipientTimezone])
INCLUDE ([Status], [PaymentStatus]);

CREATE NONCLUSTERED INDEX [IX_Orders_PreferredCallTime]
ON [dbo].[Orders] ([PreferredCallTime])
INCLUDE ([Status], [RecipientTimezone]);

-- Comentario en las columnas para documentación
EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Preferred call time in HH:MM format (Aguascalientes timezone)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Orders',
    @level2type = N'COLUMN',
    @level2name = N'PreferredCallTime';

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Timezone of the recipient (CENTRO, MONTANA, PACIFICO, NOROESTE, QUINTANA_ROO)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Orders',
    @level2type = N'COLUMN',
    @level2name = N'RecipientTimezone';

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'State/region of the recipient for reference',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Orders',
    @level2type = N'COLUMN',
    @level2name = N'RecipientState';
