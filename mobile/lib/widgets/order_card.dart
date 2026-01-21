import 'package:flutter/material.dart';

class OrderCard extends StatelessWidget {
  final String recipientName;
  final String phoneNumber;
  final String message;
  final bool isAnonymous;
  final double price;
  final VoidCallback onAccept;

  const OrderCard({
    Key? key,
    required this.recipientName,
    required this.phoneNumber,
    required this.message,
    required this.isAnonymous,
    required this.price,
    required this.onAccept,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.all(8.0),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                if (isAnonymous)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: Colors.grey[300],
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: const Text('🔒 Anónimo', style: TextStyle(fontSize: 12)),
                  )
                else
                  const SizedBox(),
                Text('\$${price.toStringAsFixed(2)} MXN', 
                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
              ],
            ),
            const SizedBox(height: 12),
            Text('Receptor: $recipientName', style: const TextStyle(fontWeight: FontWeight.bold)),
            Text('Teléfono: $phoneNumber', style: const TextStyle(color: Colors.grey)),
            const SizedBox(height: 8),
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: Colors.grey[100],
                borderRadius: BorderRadius.circular(4),
              ),
              child: Text(message, maxLines: 3, overflow: TextOverflow.ellipsis),
            ),
            const SizedBox(height: 12),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: onAccept,
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.purple,
                ),
                child: const Text('Aceptar Encargo'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
